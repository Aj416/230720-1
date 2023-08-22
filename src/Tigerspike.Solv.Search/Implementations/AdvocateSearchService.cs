using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Elasticsearch.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Models.Search;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Search.Models;

namespace Tigerspike.Solv.Search.Implementations
{
	public class AdvocateSearchService : ElasticSearchService<AdvocateSearchModel>
	{
		protected override string LiveAlias => "advocates";
		private readonly IMapper _mapper;
		private readonly IAdvocateRepository _advocateRepository;

		public AdvocateSearchService(IOptions<ElasticSearchOptions> options, ILogger<ElasticSearchService<AdvocateSearchModel>> logger, IMapper mapper, IAdvocateRepository advocateRepository) : base(options, logger)
		{
			_mapper = mapper;
			_advocateRepository = advocateRepository;
		}

		public override async Task<IPagedList> Search(SearchBaseCriteriaModel baseCriteria)
		{
			var criteria = (AdvocateSearchCriteriaModel)baseCriteria;
			var criteriaStatuses = criteria.Statuses.ToEnumList<AdvocateSearchStatus>();
			var sortOrder = (SortOrder)(int)criteria.SortOrder;
			var brandId = criteria.BrandId.ToString();

			var response = await Client.SearchAsync<AdvocateSearchModel>(
				s =>
				{
					s.Index(LiveAlias);
					s.Query(q =>
						{
							var qc = q.MultiMatch(m =>
							m.Fields(fd =>
								{
									var x = fd.Fields(
										p => p.FullName,
										p => p.Brands[0].Csat
									);

									if (!criteria.BrandId.HasValue)
									{
										x.Fields(p => p.UnAuthorisedBrandNames);
										x.Fields(p => p.InvitedStatus);
										x.Fields(p => p.BlockBrandNames);
									}

									return x;
								})
							.Operator(Operator.And)
							.Query(criteria.Term)
							.Type(TextQueryType.PhrasePrefix).Lenient());

							// Filter results by either the brand status or to the advocate status (depenends on whether is a client or admin is requesting the search).
							var statusCriteria = FilterBrandCriterion(q, criteriaStatuses, criteria.BrandId) ?? FilterAdvocateStatuses(q, criteriaStatuses);

							if (statusCriteria != null)
							{
								qc &= statusCriteria;
							}

							return qc;
						})
						.Size(criteria.PageSize)
						.Skip(criteria.PageIndex * criteria.PageSize);

					//Check if the 'sort by' is supplied, currently the front-end is not sending it which it should.
					if (criteria.SortBy != AdvocateSortBy.unspecified)
					{
						//TODO: Remove the following condition when finding the solution to https://discuss.elastic.co/t/case-insensitive-sort-doesnt-work/143192
						//This is just a workaround to sort by display name
						if (criteria.SortBy.ToString().Equals(nameof(AdvocateSearchModel.FullName), StringComparison.InvariantCultureIgnoreCase))
						{
							// Use NameSortToken instead of actual display name
							s.Sort(sr => sr.Field("nameSortToken", sortOrder));
						}
						else if (criteria.SortBy == AdvocateSortBy.onboarding)
						{
							// Use NameSortToken instead of actual display name
							s.Sort(sr => sr.Field(nameof(AdvocateSearchModel.OnboardingItemsCompleted).ToCamelCase(), sortOrder));
						}
						else if (criteria.SortBy.ToString().Equals(nameof(AdvocateSearchModel.Status), StringComparison.InvariantCultureIgnoreCase))
						{
							// If the sort column is status, we need to dive deep into the brands collection
							// since status property is nested there.
							if (criteria.BrandId != null)
							{
								s.Sort(sr => sr.Field(f => f.Field(fi => fi.Brands[0].StatusText)
									.Order(sortOrder)
									.Nested(n => n.Path(p => p.Brands).Filter(fl => fl.Term(t => t.Field(tf => tf.Brands[0].BrandId).Value(brandId))))));
							}
							else
							{
								s.Sort(sr => sr.Field(f => f.Field(fi => fi.StatusText)
									.Order(sortOrder)));
							}
						}
						else if (criteria.SortBy.ToString().Equals(nameof(AdvocateSearchModel.Csat), StringComparison.InvariantCultureIgnoreCase))
						{
							// If the sort column is csat, we need to dive deep into the brands collection
							// since csat s property is nested there.
							if (criteria.BrandId != null)
							{
								s.Sort(sr => sr.Field(f => f.Field(fi => fi.Brands[0].Csat)
									.Order(sortOrder)
									.Nested(n => n.Path(p => p.Brands).Filter(fl => fl.Term(t => t.Field(tf => tf.Brands[0].BrandId).Value(brandId))))));
							}
							else
							{
								s.Sort(sr => sr.Field(f => f.Field(fi => fi.Csat)
									.Order(sortOrder)));
							}
						}
						else if (criteria.SortBy.ToString().Equals(nameof(AdvocateSearchModel.UnAuthorisedBrandNames), StringComparison.InvariantCultureIgnoreCase))
						{
							s.Sort(sr => sr.Field(srf =>
							{
								srf.Field(p => p.UnAuthorisedBrandNames.Suffix("keyword")).Order((SortOrder)(int)criteria.SortOrder);
								srf.Missing(-1);
								return srf;
							}));
						}
						else if (criteria.SortBy.ToString().Equals(nameof(AdvocateSearchModel.BlockBrandNames), StringComparison.InvariantCultureIgnoreCase))
						{
							s.Sort(sr => sr.Field(srf =>
							{
								srf.Field(p => p.BlockBrandNames.Suffix("keyword")).Order((SortOrder)(int)criteria.SortOrder);
								srf.Missing(-1);
								return srf;
							}));
						}
						else if (criteria.SortBy.ToString().Equals(nameof(AdvocateSearchModel.InvitedStatus), StringComparison.InvariantCultureIgnoreCase))
						{
							s.Sort(sr => sr.Field(srf =>
							{
								srf.Field(p => p.InvitedStatus.Suffix("keyword")).Order((SortOrder)(int)criteria.SortOrder);
								srf.Missing(-1);
								return srf;
							}));
						}
						else
						{
							s.Sort(sr => sr.Field(new Field(criteria.SortBy.ToString()), sortOrder)).Suffix("keword");
						}
					}

					// Select fields to be returned.
					s.Source(sr => sr.Includes(i => i.Fields(f => f.Id, f => f.FullName, f => f.Status, f => f.Csat, f => f.Brands, f => f.IdentityVerificationStatus, f => f.PracticeComplete, f => f.VideoWatched, f => f.PaymentMethodSetup, f => f.PaymentEmailVerified, f => f.QuizAttempts)));

					return s;
				}
			);

			if (!response.IsValid)
			{
				throw new InvalidOperationException(response.DebugInformation);
			}

			var result = response.Documents.ToList();

			if (criteria.BrandId.HasValue)
			{
				// If the results are filtered by a brand, we should fill the status of this advocate in relation to the brand (for client view).
				foreach (var item in result)
				{
					var brand = item.Brands.Single(b => b.BrandId == criteria.BrandId);
					item.BrandStatus = brand.StatusText;
					item.Csat = brand.Csat;
				}
			}

			return PagedList.FromExisting(result.Select(_mapper.Map<AdvocateSearchResponseModel>).ToList(), criteria.PageIndex, criteria.PageSize, response.Total, 0);
		}

		private QueryContainer FilterBrandCriterion(QueryContainerDescriptor<AdvocateSearchModel> q, IList<AdvocateSearchStatus> statuses, Guid? brandId)
		{
			var authorizedFilters = new[] { AdvocateSearchStatus.Authorized, AdvocateSearchStatus.Unauthorized };
			var terms = new List<QueryContainer>();
			QueryContainer qc = null;

			if (brandId.HasValue)
			{
				terms.Add(q.Term(t => t.Field(tf => tf.Brands[0].BrandId).Value(brandId.Value)));
			}

			if (statuses.Intersect(authorizedFilters).Any())
			{
				// Filtering authorized/unauthroized
				var authorizedFlag = statuses.Contains(AdvocateSearchStatus.Authorized);
				terms.Add(q.Term(t => t.Field(tf => tf.Brands[0].Authorized).Value(authorizedFlag.ToLower())));

				if (authorizedFlag)
				{
					// if we are filtering Authorized advocates, we implicitly saying they should be enabled as well (otherwise they are Idle)
					terms.Add(q.Term(t => t.Field(tf => tf.Brands[0].Enabled).Value("true")));
					terms.Add(q.Term(t => t.Field(tf => tf.Brands[0].Blocked).Value("false")));
				}
			}

			if (statuses.Contains(AdvocateSearchStatus.Idle))
			{
				// Filtering idle solvers (who are authorized but disabled the brand)
				terms.Add(q.Term(t => t.Field(tf => tf.Brands[0].Authorized).Value("true")));
				terms.Add(q.Term(t => t.Field(tf => tf.Brands[0].Enabled).Value("false")));
			}

			if (statuses.Contains(AdvocateSearchStatus.Blocked))
			{
				terms.Add(q.Term(t => t.Field(tf => tf.Brands[0].Blocked).Value("true")));
			}

			if (terms.Any())
			{
				qc &= q.Nested(ns => ns.Path(p => p.Brands).Query(nq => nq.Bool(b => b.Must(terms.ToArray()))));
			}

			return qc;
		}

		private QueryContainer FilterAdvocateStatuses(QueryContainerDescriptor<AdvocateSearchModel> q, IList<AdvocateSearchStatus> statuses)
		{
			var advocateFilters = new[] { AdvocateSearchStatus.Verified, AdvocateSearchStatus.Unverified, AdvocateSearchStatus.Blocked };
			var statusesForAdvocateFilters = statuses.Intersect(advocateFilters).ToList();
			if (statusesForAdvocateFilters.Any())
			{
				var queryString = string.Join(" ", statusesForAdvocateFilters);
				return q.Match(m => m.Field(p => p.StatusText).Query(queryString).Operator(Operator.Or));
			}

			return null;
		}

		public override async Task Update(Guid id, object partialAdvocate) =>
			await Client.UpdateAsync<AdvocateSearchModel, object>(DocumentPath<AdvocateSearchModel>.Id(id),
				u => u.Index(LiveAlias).Doc(partialAdvocate).Refresh(Refresh.True));

		protected override Func<CreateIndexDescriptor, ICreateIndexRequest> IndexDescriptor() => (x => x
			.Aliases(a => a.Alias(LiveAlias))
			.Mappings(m => m.Map<AdvocateSearchModel>(tm => tm
				.AutoMap()
				.Properties(p => p
					.Text(k => k.Name(nm => nm.FullName) /*.Analyzer("lowercase_analyzer") */ .Fields(fd => fd.Keyword(kw => kw.Name(nm => nm.FullName.Suffix("keyword")))))
					.Text(k => k.Name(nm => nm.Status).Fielddata())
					.Text(k => k.Name(nm => nm.StatusText).Fielddata())
					.Keyword(kw => kw.Name(nm => nm.NameSortToken))
					.Nested<AdvocateBrandSearchModel>(n => n.Name(nm => nm.Brands)
						.AutoMap()
						.Properties(np => np.Keyword(kw => kw.Name(kn => kn.StatusText))))
					.Nested<AdvocateQuizSearchModel>(n => n.Name(nm => nm.QuizAttempts)
						.AutoMap()))
			))
			.Settings(s => s
					.Analysis(a => a
						.Analyzers(aa => aa
							.Custom("lowercase_analyzer", ca => ca
								.Tokenizer("keyword")
								.Filters("lowercase")
							)
						)
					)
			)
		);

		protected override async Task<long> FillIndex(string indexName)
		{
			var page = PagedList.ForLoop<Advocate>();

			do
			{
				page = await _advocateRepository.GetAdvocatesForIndexing(page.PageIndex + 1);
				var advocates = page.Items.Select(_mapper.Map<AdvocateSearchModel>).ToArray();
				await Index(indexName, advocates);

			} while (page.HasNextPage);

			return page.TotalCount;
		}
	}
}