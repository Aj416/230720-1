using System;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.Logging;
using Nest;

namespace Tigerspike.Solv.Search.Extensions
{

	public static class NestExtensionMethods
	{
		public const string IndexPartialAnalyzer = nameof(IndexPartialAnalyzer);
		public const string SearchPartialAnalyzer = nameof(SearchPartialAnalyzer);

		public static void Verbose(this ConnectionSettings settings, ILogger logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("Logger should be specified");
			}

			settings
				// pretty print all requests and responses to Elasticsearch
				.PrettyJson()
				// buffer the request and response bytes in MemoryStream
				.DisableDirectStreaming()
				// a delegate to run when a request completes. Use this to capture
				// request and response information
				.OnRequestCompleted(response =>
				{
					// log out the request
					if (response.RequestBodyInBytes != null)
					{
						logger.LogDebug(
							$"{response.HttpMethod} {response.Uri} \n" +
							$"{Encoding.UTF8.GetString(response.RequestBodyInBytes)}");
					}
					else
					{
						logger.LogDebug($"{response.HttpMethod} {response.Uri}");
					}

					logger.LogDebug("\n");

					// log out the response
					if (response.ResponseBodyInBytes != null)
					{
						logger.LogDebug($"Status: {response.HttpStatusCode}\n" +
							$"{Encoding.UTF8.GetString(response.ResponseBodyInBytes)}\n" +
							$"{new string('-', 30)}\n");
					}
					else
					{
						logger.LogDebug($"Status: {response.HttpStatusCode}\n" +
							$"{new string('-', 30)}\n");
					}
				});
		}

		public static TextPropertyDescriptor<T> Partial<T>(this TextPropertyDescriptor<T> descriptor) where T: class
		{
			return descriptor
				.Analyzer(IndexPartialAnalyzer)
				.SearchAnalyzer(SearchPartialAnalyzer);
		}

		public static IndexSettingsDescriptor PartialSearchAnalyzer(this IndexSettingsDescriptor descriptor)
		{
			return descriptor.Analysis(a => a
					.Analyzers(analyzer => analyzer
						.Custom(IndexPartialAnalyzer, analyzerDescriptor => analyzerDescriptor
							.Tokenizer("standard")
							.Filters("lowercase", nameof(PartialSearchAnalyzer))
						)
						.Custom(SearchPartialAnalyzer, analyzerDescriptor => analyzerDescriptor
							.Tokenizer("standard")
							.Filters("lowercase")
						)
					)
					.TokenFilters(tf => tf
						.NGram(nameof(PartialSearchAnalyzer), filterDescriptor => filterDescriptor
							.MinGram(1)
							.MaxGram(15)
						)
					)
			);
		}

		public static QueryContainer QueryNested<T>(this QueryContainerDescriptor<T> qd, Expression<Func<T, object>> path, Expression<Func<T, object>> field, string value)
			where T : class => qd.Nested(n => n.Path(path).Query(nq => nq.Term(m => m.Field(field).Value(value))));
	}
}
