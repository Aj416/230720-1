using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Polly;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Models.Search;
using Tigerspike.Solv.Services.Fraud.Configuration;
using Tigerspike.Solv.Services.Fraud.Enum;
using Tigerspike.Solv.Services.Fraud.Extensions;
using Tigerspike.Solv.Services.Fraud.Models;

namespace Tigerspike.Solv.Services.Fraud.Application.Services
{

	public abstract class ElasticSearchService<T> : ISearchService<T> where T : class
	{
		protected abstract string LiveAlias { get; }
		protected IElasticClient Client { get; }
		private readonly ILogger<ElasticSearchService<T>> _logger;

		protected ElasticSearchService(IOptions<ElasticSearchOptions> options, ILogger<ElasticSearchService<T>> logger)
		{
			var settings = new ConnectionSettings(new Uri(options.Value.Uri))
				.ThrowExceptions(true);

			settings.Verbose(logger);

			if (options.Value.UserName.IsNotEmpty())
			{
				settings.BasicAuthentication(options.Value.UserName, options.Value.Password);
			}

			Client = new ElasticClient(settings);
			_logger = logger;
		}

		public async Task Index(T item)
		{
			if (item != null)
			{
				await Client.IndexAsync(item, x => x.Index(LiveAlias));
			}
		}

		protected async Task Index(string indexName, params T[] items)
		{
			if (items?.Length > 0)
			{
				var maxRetryAttempts = 5;
				var baseRetryWait = TimeSpan.FromSeconds(3);
				var expotentialBackoffPolicy = Polly.Policy
					.Handle<Exception>()
					.WaitAndRetryAsync(maxRetryAttempts, attempt =>
					{
						_logger.LogWarning($"Bulk index on {indexName} failed (waiting to retry {attempt}/{maxRetryAttempts})");
						return TimeSpan.FromSeconds(Math.Pow(baseRetryWait.TotalSeconds, attempt));
					});

				await expotentialBackoffPolicy.ExecuteAsync(async () =>
				{
					await Client.BulkAsync(x => x
						.Index(indexName)
						.IndexMany(items)
						.Refresh(Elasticsearch.Net.Refresh.False)
					);
				});
			}
		}

		public Task Delete(Guid id) => Client.DeleteAsync(DocumentPath<T>.Id(id).Index(LiveAlias));

		public async Task<long> Count(string indexName) => (await Client.CountAsync<T>(x => x.Index(indexName))).Count;

		public async Task<RebuildResult> Rebuild()
		{
			try
			{
				var newIndex = await CreateIndex();
				var sourceDocumentsCount = await FillIndex(newIndex);
				await MakeLive(newIndex);
				var result = new RebuildResult()
				{
					IndexName = newIndex,
					SourceDocuments = sourceDocumentsCount,
					IndexedDocuments = await Count(newIndex)
				};

				_logger.Log(
					result.IndexedDocuments == result.SourceDocuments ? Microsoft.Extensions.Logging.LogLevel.Information : Microsoft.Extensions.Logging.LogLevel.Warning,
					"Rebuilding index [{index}] is completed {@rebuildResult}",
					LiveAlias, result
				);

				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to rebuild index");
				throw;
			}
		}

		private async Task<string> CreateIndex()
		{
			try
			{
				// create new, uniquely named index
				var indexName = $"{LiveAlias}-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
				await Client.CreateIndexAsync(indexName, IndexDescriptor());
				_logger.LogInformation($"New search index created [{indexName}]");
				return indexName;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create index");
				throw;
			}
		}

		private async Task MakeLive(string indexName)
		{
			try
			{
				_logger.LogInformation($"Refreshing search index [{indexName}]");
				// make sure index is ready to go live
				await Client.RefreshAsync(indexName);

				// get list of indexes to delete
				var outdatedIndexes = await Client.GetIndexAsync($"{LiveAlias}*");
				var toRemove = outdatedIndexes.Indices.Keys.Where(x => x != indexName).ToList();

				// check if current alias is actually an index (obsolete approach)
				// we have to delete that index straight-away, since that would prevent setting alias on new index
				if (toRemove.Contains(LiveAlias))
				{
					await Client.DeleteIndexAsync(LiveAlias);
					toRemove = toRemove.Where(x => x != LiveAlias).ToList();
				}

				// atomicly swap alias to new index
				var res = await Client.AliasAsync(aliases => aliases
					.Remove(a => a.Alias(LiveAlias).Index("*"))
					.Add(a => a.Alias(LiveAlias).Index(indexName))
				);

				_logger.LogInformation($"Search index [{indexName}] is now available under alias [{LiveAlias}]");

				// drop outdated indexes
				foreach (var idx in toRemove)
				{
					await Client.DeleteIndexAsync(idx);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to make live index {indexName}");
				throw;
			}
		}

		/// <inheritdoc />
		public async Task<long> GetCount(FraudStatus fraudStatus)
		{
			var response = await Client.CountAsync<FraudSearchModel>(c => c
				.Index(LiveAlias)
				.Query(q => q
					.Match(m => m
						.Field(p => p.FraudStatus)
						.Query(((int)fraudStatus).ToString())
					)
				)
			);

			return response.Count;
		}

		public abstract Task<IPagedList> Search(SearchBaseCriteriaModel searchCriteria);
		public abstract Task Update(Guid id, object partialItem);
		protected abstract Task<long> FillIndex(string indexName);
		protected abstract Func<CreateIndexDescriptor, ICreateIndexRequest> IndexDescriptor();

	}
}
