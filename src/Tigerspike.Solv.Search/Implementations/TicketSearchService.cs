using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Models.Search;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Search.Extensions;
using Tigerspike.Solv.Search.Models;

namespace Tigerspike.Solv.Search.Implementations
{

	public class TicketSearchService : ElasticSearchService<TicketSearchModel, Guid>
	{
		protected override string LiveAlias => "tickets";
		private readonly ITicketRepository _ticketRepository;
		private readonly IMapper _mapper;

		public TicketSearchService(
			IOptions<ElasticSearchOptions> options,
			ILogger<ElasticSearchService<TicketSearchModel, Guid>> logger,
			ITicketRepository ticketRepository,
			IMapper mapper) : base(options, logger)
		{
			_ticketRepository = ticketRepository;
			_mapper = mapper;
		}

		public override async Task<IPagedList> Search(SearchBaseCriteriaModel searchCriteria)
		{
			var criteria = (TicketSearchCriteriaModel)searchCriteria;
			var statuses = criteria.Statuses.Split(',');
			var status = statuses.FirstOrDefault();
			var dateField = GetDateField(status, false);
			var dateFieldText = GetDateField(status, true);

			var result = await Client.SearchAsync<TicketSearchModel>(
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
									p => p.Question,
									p => p.Price,
									p => p.Source,
									p => p.ReferenceId,
									p => p.EscalationReasonText,
									p => p.AdvocateFullName,
									p => p.BrandName,
									p => p.Metadata["*"],
									p => p.Customer.FirstName,
									p => p.Customer.LastName,
									p => p.Customer.DisplayName,
									p => p.Customer.Email
								);


								if (dateFieldText != null)
								{
									fd.Fields(dateFieldText);
								}

								return fd;
							})
							.Operator(Operator.And)
							.Query(criteria.Term)
							.Type(TextQueryType.BestFields).Lenient());

						if (criteria.AdvocateId != null)
						{
							qc &= q.Match(m => m.Field(p => p.AdvocateId).Query(criteria.AdvocateId.ToString()));

							if(criteria.Statuses.Contains(nameof(TicketStatusEnum.Closed)))
							{
								qc &= !q.Match(m => m.Field(p => p.FraudStatusText).Query(nameof(FraudStatus.FraudConfirmed)));
							}
						}

						if (criteria.BrandId.HasValue)
						{
							qc &= q.Match(m => m.Field(p => p.BrandId).Query(criteria.BrandId.ToString()));
						}

						if (criteria.Level != null)
						{
							qc &= q.Match(m => m.Field(p => p.Level).Query(((int)criteria.Level.Value).ToString()));
						}

						if (!string.IsNullOrEmpty(criteria.Statuses))
						{
							qc &= q.Match(m =>
								m.Field(p => p.StatusText)
									.Query(criteria.Statuses));
						}

						if (criteria.From.HasValue && criteria.To.HasValue)
						{
							qc &= q.DateRange(dr => dr.Field(dateField).GreaterThanOrEquals(criteria.From.Value).LessThanOrEquals(criteria.To ?? DateTime.UtcNow));
						}

						qc &= q.Match(m => 
							m.Field(p => p.Ready)
								.Query("true"));
						
						return qc;
					})
						.Size(criteria.PageSize)
						.Skip(criteria.PageIndex * criteria.PageSize);
					if (criteria.SortBy != TicketSortBy.unspecified)
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
					return s;
				}
			);

			return PagedList.FromExisting(result.Documents.ToList(), criteria.PageIndex, criteria.PageSize, result.Total, 0);
		}

		private string GetSortingField(TicketSortBy sortBy)
		{
			switch (sortBy)
			{
				case TicketSortBy.advocateFullName:
					return nameof(TicketSearchModel.AdvocateFullNameSortToken);
				case TicketSortBy.escalationReasonText:
					return nameof(TicketSearchModel.EscalationReasonSortToken);
				case TicketSortBy.brandName:
					return nameof(TicketSearchModel.BrandNameSortToken);
				case TicketSortBy.timeOpen:
					return nameof(TicketSearchModel.CreatedDate);
				default:
					return sortBy.ToString();
			}
		}

		/// <summary>
		/// Return the date field expression (in text format if specified) that corresponds to a ticket status.
		/// </summary>
		private Expression<Func<TicketSearchModel, object>> GetDateField(string ticketStatus, bool useText)
		{
			Expression<Func<TicketSearchModel, object>> createdDateField = p => p.CreatedDate;
			Expression<Func<TicketSearchModel, object>> createdDateFieldText = p => p.CreatedDateText;

			Expression<Func<TicketSearchModel, object>> closedDateField = p => p.ClosedDate;
			Expression<Func<TicketSearchModel, object>> closedDateFieldText = p => p.ClosedDateText;

			Expression<Func<TicketSearchModel, object>> escalatedDateField = p => p.EscalatedDate;
			Expression<Func<TicketSearchModel, object>> escalatedDateFieldText = p => p.EscalatedDateText;

			switch (ticketStatus)
			{
				case nameof(TicketStatusEnum.New):
				case nameof(TicketStatusEnum.Assigned):
				case nameof(TicketStatusEnum.Reserved):
					return useText ? createdDateFieldText : createdDateField;
				case nameof(TicketStatusEnum.Solved):
				case nameof(TicketStatusEnum.Closed):
					return useText ? closedDateFieldText : closedDateField;
				case nameof(TicketStatusEnum.Escalated):
					return useText ? escalatedDateFieldText : escalatedDateField;
				default:
					throw new Exception("Unrecognized status");
			}
		}



		public override async Task Update(Guid id, object partialTicket)
		{
			await Client.UpdateAsync<TicketSearchModel, object>(DocumentPath<TicketSearchModel>.Id(id),
					u => u
					.Index(LiveAlias)
					.Doc(partialTicket)
					.Upsert(new TicketSearchModel { Id = id })
					.Refresh(Refresh.True)
					.RetryOnConflict(10)
				);
		}

		protected override Func<CreateIndexDescriptor, ICreateIndexRequest> IndexDescriptor() => (x => x
			.Aliases(a => a.Alias(LiveAlias))
			.Mappings(m => m.Map<TicketSearchModel>(tm => tm
				.AutoMap()
				.DynamicTemplates(dt => dt
					.DynamicTemplate(nameof(TicketSearchModel.Metadata), dtd => dtd
						.PathMatch(nameof(TicketSearchModel.Metadata) + ".*")
						.Mapping(dm => dm
							.Text(k => k.Name("keyword").Partial())
						)
					)
				)
				.Properties(p => p
					.Number(nm => nm.Name(n => n.AbsoluteTimeToClose).NullValue(0))
					.Number(nm => nm.Name(n => n.SolverTimeToClose).NullValue(0))
					.Number(nm => nm.Name(n => n.Price).NullValue(0))
					.Number(nm => nm.Name(n => n.Complexity).NullValue(-1).Type(NumberType.Integer))
					.Number(nm => nm.Name(n => n.Csat).NullValue(-1).Type(NumberType.Integer))
					.Text(k => k.Name(nm => nm.Id).Partial())
					.Text(k => k.Name(nm => nm.AdvocateFullName).Partial())
					.Text(k => k.Name(nm => nm.BrandName).Partial())
					.Text(k => k.Name(nm => nm.Source).Partial())
					.Text(k => k.Name(nm => nm.ReferenceId).Partial())
					.Text(k => k.Name(nm => nm.Question).Partial())
					.Text(k => k.Name(nm => nm.EscalationReasonText).Partial())
					.Object<Dictionary<string, string>>(o => o.Name(n => n.Metadata))
					.Object<UserModel>(um => um
						.Name(x => x.Customer)
						.Properties(np => np
							.Text(k => k.Name(nm => nm.FirstName).Partial())
							.Text(k => k.Name(nm => nm.LastName).Partial())
							.Text(k => k.Name(nm => nm.DisplayName).Partial())
							.Text(k => k.Name(nm => nm.Email).Partial())
						)
					)
					.Keyword(kw => kw.Name(nm => nm.AdvocateFullNameSortToken))
					.Keyword(kw => kw.Name(nm => nm.EscalationReasonSortToken))
					.Keyword(kw => kw.Name(nm => nm.BrandNameSortToken))
				)
			))
			.Settings(s => s.PartialSearchAnalyzer())
		);

		protected override async Task<long> FillIndex(string indexName)
		{
			var page = PagedList.ForLoop<TicketSearchModel>();

			do
			{
				page = await _ticketRepository.GetPagedListAsync<TicketSearchModel>(
					_mapper,
					pageIndex: page.PageIndex + 1,
					predicate: t => !t.IsPractice,
					orderBy: t => t.OrderBy(x => x.CreatedDate));

				var tasks = new List<Task>();

				foreach (var ticket in page.Items)
				{
					//TODO: A workaround for sorting, should be removed when we figure out how to sort null values using ElasticSearch
					//As currently, it puts them at the bottom always.
					ticket.Csat = ticket.Csat ?? 0;
					ticket.Complexity = ticket.Complexity ?? 0;
					//ticket.Price = ticket.Price ?? 0;
				}

				await Index(indexName, page.Items.ToArray());

			} while (page.HasNextPage);

			return page.TotalCount;
		}

	}
}
