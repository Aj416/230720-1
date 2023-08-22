using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Models.Search;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Search.Models;

namespace Tigerspike.Solv.Search.Implementations
{
	public class AdvocateApplicationSearchService : ElasticSearchService<AdvocateApplicationSearchModel>
	{

		protected override string LiveAlias => "advocateapplications";
		private readonly IMapper _mapper;
		private readonly IAdvocateApplicationRepository _advocateApplicationRepository;


		public AdvocateApplicationSearchService(IOptions<ElasticSearchOptions> options, ILogger<ElasticSearchService<AdvocateApplicationSearchModel>> logger, IMapper mapper, IAdvocateApplicationRepository advocateApplicationRepository) : base(options, logger)
		{
			_mapper = mapper;
			_advocateApplicationRepository = advocateApplicationRepository;
		}

		public override async Task<IPagedList> Search(SearchBaseCriteriaModel baseCriteria)
		{
			var criteria = (AdvocateApplicationSearchCriteriaModel)baseCriteria;
			var sortOrder = (SortOrder)(int)criteria.SortOrder;
			var dateField = GetDateField(criteria.Statuses.ToString(), false);
			var dateFieldText = GetDateField(criteria.Statuses.ToString(), true);

			var response = await Client.SearchAsync<AdvocateApplicationSearchModel>(
				s =>
				{
					s.Index(LiveAlias);
					s.Query(q =>
						{
							var qc = q.MultiMatch(m =>
										m.Fields(f =>
										{
											var fd = f.Fields(
												p => p.FullName,
												p => p.Country,
												p => p.Email,
												p => p.Phone,
												p => p.Source,
												p => p.Language,
												p => p.Skills.SelectMany(x => x.Display)
										);

											if (dateFieldText != null)
											{
												fd.Fields(dateFieldText);
											}

											return fd;
										})
								.Type(TextQueryType.PhrasePrefix).Lenient()
								.Query(criteria.Term));

							if (criteria.Statuses != null)
							{
								qc &= q.Match(m => m.Field(p => p.ApplicationStatus).Query(((int)criteria.Statuses.Value).ToString()));
							}

							if (criteria.Countries != null)
							{
								qc &= q.Match(m => m.Field(p => p.Country).Query(criteria.Countries));
							}

							if (criteria.Email != null)
							{
								qc &= q.Match(m => m.Field(p => p.Email).Query(criteria.Email).Analyzer("anz"));
							}

							if (criteria.Phone != null)
							{
								qc &= q.Match(m => m.Field(p => p.Phone).Query(criteria.Phone));
							}

							if (criteria.From.HasValue && criteria.To.HasValue)
							{
								qc &= q.DateRange(dr => dr.Field(dateField).GreaterThanOrEquals(criteria.From.Value).LessThanOrEquals(criteria.To ?? DateTime.UtcNow));
							}
							return qc;
						})
						.Size(criteria.PageSize)
						.Skip(criteria.PageIndex * criteria.PageSize);

					if (criteria.SortBy != AdminAdvocateApplicationStatusSortBy.Unspecified)
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
					s.Source(sr => sr.Includes(i => i.Fields(
						f => f.Id,
						f => f.FirstName,
						f => f.LastName,
						f => f.Country,
						f => f.Source,
						f => f.CreatedDate,
						f => f.Questionnaire,
						f => f.InvitationDate,
						f => f.Language,
						f => f.Skills,
						f => f.Email,
						f => f.Phone
					)));

					return s;
				}
			);

			var result = response.Documents.ToList();

			return PagedList.FromExisting(result.Select(_mapper.Map<AdvocateApplicationSearchResponseModel>).ToList(), criteria.PageIndex, criteria.PageSize, response.Total, 0);
		}

		private string GetSortingField(AdminAdvocateApplicationStatusSortBy sortBy)
		{
			switch (sortBy)
			{
				case AdminAdvocateApplicationStatusSortBy.FirstName:
				case AdminAdvocateApplicationStatusSortBy.LastName:
				case AdminAdvocateApplicationStatusSortBy.FullName:
					return nameof(AdvocateApplicationSearchModel.NameSortToken);
				case AdminAdvocateApplicationStatusSortBy.Country:
					return nameof(AdvocateApplicationSearchModel.CountrySortToken);
				case AdminAdvocateApplicationStatusSortBy.Source:
					return nameof(AdvocateApplicationSearchModel.SourceSortToken);
				case AdminAdvocateApplicationStatusSortBy.CompletedEmailSent:
					return nameof(AdvocateApplicationSearchModel.Questionnaire);
				case AdminAdvocateApplicationStatusSortBy.Language:
					return nameof(AdvocateApplicationSearchModel.LanguageSortToken);
				case AdminAdvocateApplicationStatusSortBy.Skills:
					return nameof(AdvocateApplicationSearchModel.SkillsSortToken);
				default:
					return sortBy.ToString();
			}
		}

		private Expression<Func<AdvocateApplicationSearchModel, object>> GetDateField(string applicationStatus, bool useText)
		{
			Expression<Func<AdvocateApplicationSearchModel, object>> createdDateField = p => p.CreatedDate;
			Expression<Func<AdvocateApplicationSearchModel, object>> createdDateFieldText = p => p.CreatedDateText;

			Expression<Func<AdvocateApplicationSearchModel, object>> invitationDateField = p => p.InvitationDate;
			Expression<Func<AdvocateApplicationSearchModel, object>> invitationDateFieldText = p => p.InvitationDateText;

			switch (applicationStatus)
			{
				case nameof(AdvocateApplicationStatus.New):
				case nameof(AdvocateApplicationStatus.NotSuitable):
					return useText ? createdDateFieldText : createdDateField;
				case nameof(AdvocateApplicationStatus.Invited):
					return useText ? invitationDateFieldText : invitationDateField;
				default:
					throw new Exception("Unrecognized status");
			}
		}

		protected override Func<CreateIndexDescriptor, ICreateIndexRequest> IndexDescriptor() => (x => x
			.Aliases(a => a.Alias(LiveAlias))
			.Mappings(m => m.Map<AdvocateApplicationSearchModel>(tm => tm
				.AutoMap()
				.Properties(p => p
					.Keyword(kw => kw.Name(nm => nm.NameSortToken))
					.Keyword(kw => kw.Name(nm => nm.CountrySortToken))
					.Keyword(kw => kw.Name(nm => nm.SourceSortToken))
					.Keyword(kw => kw.Name(nm => nm.LanguageSortToken))
					.Keyword(kw => kw.Name(nm => nm.SkillsSortToken))
				)
			))
		);

		protected override async Task<long> FillIndex(string indexName)
		{
			var page = PagedList.ForLoop<AdvocateApplicationSearchModel>();

			do
			{
				page = await _advocateApplicationRepository.GetPagedListAsync<AdvocateApplicationSearchModel>(_mapper,
					predicate: t => t.ApplicationStatus != AdvocateApplicationStatus.AccountCreated,
					pageIndex: page.PageIndex + 1);

				await Index(indexName, page.Items.ToArray());

			} while (page.HasNextPage);

			return page.TotalCount;
		}

		public override async Task Update(Guid id, object partialAdvocateApplication) =>
			await Client.UpdateAsync<AdvocateApplicationSearchModel, object>(DocumentPath<AdvocateApplicationSearchModel>.Id(id),
				u => u.Index(LiveAlias).Doc(partialAdvocateApplication).Refresh(Refresh.True));

	}
}