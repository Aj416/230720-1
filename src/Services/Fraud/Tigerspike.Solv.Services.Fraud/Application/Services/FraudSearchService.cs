using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Models.Search;
using Tigerspike.Solv.Services.Fraud.Configuration;
using Tigerspike.Solv.Services.Fraud.Enum;
using Tigerspike.Solv.Services.Fraud.Extensions;
using Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces;
using Tigerspike.Solv.Services.Fraud.Models;

namespace Tigerspike.Solv.Services.Fraud.Application.Services
{
	public class FraudSearchService : ElasticSearchService<FraudSearchModel>
	{
		protected override string LiveAlias => "frauds";
		private readonly ITicketRepository _ticketRepository;
		private readonly IMapper _mapper;
		private readonly IFraudService _fraudService;

		public FraudSearchService(
			IOptions<ElasticSearchOptions> options,
			ILogger<ElasticSearchService<FraudSearchModel>> logger,
			IFraudService fraudService,
			ITicketRepository ticketRepository,
			IMapper mapper) : base(options, logger) => (_fraudService, _ticketRepository, _mapper) = (fraudService, ticketRepository, mapper);

		public override async Task<IPagedList> Search(SearchBaseCriteriaModel searchCriteria)
		{
			var criteria = (FraudSearchCriteriaModel)searchCriteria;
			var dateField = GetDateField(false);
			var dateFieldText = GetDateField(true);

			var result = await Client.SearchAsync<FraudSearchModel>(
			s =>
			{
				s.Index(LiveAlias);
				s.Query(q =>
				{
					var qc = q.MultiMatch(m =>
						m.Fields(f =>
						{
							var fd = f.Fields(
								p => p.Id,
								p => p.BrandName,
								p => p.LevelText,
								p => p.StatusText,
								p => p.AdvocateName,
								p => p.FraudRiskLevel,
								p => p.FraudRisks,
								p => p.CreatedDateText,
								p => p.Metadata["*"],
								p => p.CustomerDetail.FirstName,
								p => p.CustomerDetail.LastName,
								p => p.CustomerDetail.FullName,
								p => p.CustomerDetail.Email,
								p => p.Question,
								p => p.IpAddress
							);

							return fd;
						})
						.Operator(Operator.And)
						.Query(criteria.Term)
						.Type(TextQueryType.PhrasePrefix).Lenient());

					if (criteria.BrandId.HasValue)
					{
						qc &= q.Match(m => m.Field(p => p.BrandId).Query(criteria.BrandId.ToString()));
					}

					if (criteria.Statuses != null)
					{
						qc &= q.Match(m => m.Field(p => p.FraudStatus).Query(((int)criteria.Statuses.Value).ToString()));
					}

					if (criteria.From.HasValue && criteria.To.HasValue)
					{
						qc &= q.DateRange(dr => dr.Field(dateField).GreaterThanOrEquals(criteria.From.Value).LessThanOrEquals(criteria.To ?? DateTime.UtcNow));
					}
					return qc;
				})
					.Size(criteria.PageSize)
					.Skip(criteria.PageIndex * criteria.PageSize);

				if (criteria.SortBy != FraudSortBy.Unspecified)
				{
					s.Sort(sr =>
						sr.Field(p =>
						{
							var field = new Field(GetSortingField(criteria.SortBy).ToCamelCase()); //Create the filed to be sorted by
							p.Field(field).Order((SortOrder)(int)criteria.SortOrder); // Set the sort order
							p.Missing(-1); //When the values are null we treat them as a number to be sorted properly
							return p;
						}));
				}

				// Select fields to be returned.
				s.Source(sr => sr.Includes(i => i.Fields(f => f.Id, f => f.CreatedDate, f => f.BrandId, f => f.BrandName, f => f.Status, 
					f => f.FraudStatus, f => f.Level, f => f.AdvocateName, f => f.FraudRisks, f => f.FraudLevel, f => f.Metadata, f => f.IpAddress)));

				return s;
			});

			if (!result.IsValid)
			{
				throw new Exception(result.DebugInformation);
			}

			return PagedList.FromExisting(result.Documents.ToList(), criteria.PageIndex, criteria.PageSize, result.Total, 0);
		}

		/// <summary>
		/// Return the date field expression (in text format if specified).
		/// </summary>
		private Expression<Func<FraudSearchModel, object>> GetDateField(bool useText)
		{
			Expression<Func<FraudSearchModel, object>> createdDateField = p => p.CreatedDate;
			Expression<Func<FraudSearchModel, object>> createdDateFieldText = p => p.CreatedDateText;

			return useText ? createdDateFieldText : createdDateField;
		}

		private string GetSortingField(FraudSortBy sortBy)
		{
			switch (sortBy)
			{
				case FraudSortBy.Created:
					return nameof(FraudSearchModel.CreatedDate);
				case FraudSortBy.Brand:
					return nameof(FraudSearchModel.BrandSortToken);
				case FraudSortBy.Level:
					return nameof(FraudSearchModel.LevelSortToken);
				case FraudSortBy.Status:
					return nameof(FraudSearchModel.StatusSortToken);
				case FraudSortBy.Assignee:
					return nameof(FraudSearchModel.AdvocateSortToken);
				case FraudSortBy.Risk:
					return nameof(FraudSearchModel.FraudRiskLevelSortToken);
				default:
					return sortBy.ToString();
			}
		}

		public override async Task Update(Guid id, object partialItem)
		{
			await Client.UpdateAsync<FraudSearchModel, object>(DocumentPath<FraudSearchModel>.Id(id),
					u => u
					.Index(LiveAlias)
					.Doc(partialItem)
					.Upsert(new FraudSearchModel { Id = id })
					.Refresh(Refresh.True)
					.RetryOnConflict(10)
				);
		}

		protected override async Task<long> FillIndex(string indexName)
		{
			var page = PagedList.ForLoop<Guid>();
			var ticketIds = await _ticketRepository.GetFraudTicketIds();

			do
			{
				page = ticketIds.ToPagedList(page.PageIndex + 1, 20);
				var fraudSearchModels = page.Items.Select(tid => _mapper.Map<FraudSearchModel>(_fraudService.GetTicketDetails(tid)));
				await Index(indexName, fraudSearchModels.ToArray());

			} while (page.HasNextPage);

			return page.TotalCount;
		}

		protected override Func<CreateIndexDescriptor, ICreateIndexRequest> IndexDescriptor() => x => x
			.Aliases(a => a.Alias(LiveAlias))
			.Mappings(m => m.Map<FraudSearchModel>(tm => tm
			   .AutoMap()
			   .DynamicTemplates(dt => dt
				   .DynamicTemplate(nameof(FraudSearchModel.Metadata), dtd => dtd
					   .PathMatch(nameof(FraudSearchModel.Metadata) + ".*")
					   .Mapping(dm => dm
						   .Text(k => k.Name("keyword").Partial())
					   )
				   )
			   )
			   .Properties(p => p
					 .Text(k => k.Name(nm => nm.Id).Partial())
					 .Text(k => k.Name(nm => nm.AdvocateName).Partial())
					 .Text(k => k.Name(nm => nm.BrandName).Partial())
					 .Text(k => k.Name(nm => nm.Question).Partial())
					 .Object<IDictionary<string, string>>(o => o.Name(n => n.Metadata))
					 .Object<CustomerModel>(cm => cm
					 	.Name(x => x.CustomerDetail)
					 	.Properties(np => np
					 		.Text(k => k.Name(nm => nm.FirstName).Partial())
					 		.Text(k => k.Name(nm => nm.LastName).Partial())
					 		.Text(k => k.Name(nm => nm.Email).Partial())
							.Text(k => k.Name(nm => nm.FullName).Partial())
					 	)
					 )
					.Keyword(kw => kw.Name(nm => nm.BrandSortToken))
					.Keyword(kw => kw.Name(nm => nm.LevelSortToken))
					.Keyword(kw => kw.Name(nm => nm.StatusSortToken))
					.Keyword(kw => kw.Name(nm => nm.AdvocateSortToken))
					.Keyword(kw => kw.Name(nm => nm.FraudRiskLevelSortToken))
			   )
		   ))
		   .Settings(s => s.PartialSearchAnalyzer());
	}
}
