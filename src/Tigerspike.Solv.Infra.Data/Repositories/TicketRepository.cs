using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Z.EntityFramework.Plus;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class TicketRepository : Repository<Ticket>, ITicketRepository
	{
		private readonly ITimestampService _timestampService;
		private readonly ILogger<TicketRepository> _logger;
		public const string SerialNumber = "serialNumber";

		public TicketRepository(SolvDbContext context, ITimestampService timestampService, ILogger<TicketRepository> logger) : base(context)
		{
			_timestampService = timestampService;
			_logger = logger;
		}

		#region GetAverageTimeToRespond

		private async Task<double?> GetAverageTimeToRespond(IQueryable<Ticket> queryable, DateTime? from = null, DateTime? to = null)
		{
			queryable = queryable.Where(x => x.FirstMessageDate.HasValue);

			queryable = GetTicketsInDateRange(queryable, from, to);

			return (await queryable
				.Select(x => x.FirstMessageDate - x.CreatedDate)
				.Select(x => x.Value.TotalSeconds)
				.ToListAsync()) // as EF3.1 / Pomelo can't properly calculate Average on SQL side, as a workaround we fetch the results and calculate it locally
				.Select(x => (double?)(x))
				.DefaultIfEmpty(null)
				.Average();
		}

		/// <inheritdoc/>
		public async Task<double?> GetAverageTimeToRespondForAll(DateTime? from = null, DateTime? to = null) => await GetAverageTimeToRespond(GetQueryableForAll(), from, to);

		/// <inheritdoc/>
		public async Task<double?> GetAverageTimeToRespondForBrand(Guid brandId, DateTime? from = null, DateTime? to = null) => await GetAverageTimeToRespond(GetQueryableForBrand(brandId), from, to);

		#endregion

		#region GetFirstAttemptSuccessRate

		private async Task<int?> GetFirstAttemptSuccessRate(IQueryable<Ticket> queryable, DateTime? from = null, DateTime? to = null)
		{
			queryable = queryable.Where(x => x.Status == TicketStatusEnum.Closed);
			queryable = GetTicketsInDateRange(queryable, from, to);

			var abandonedGroups = await queryable
				.GroupBy(x => new { Abandoned = x.AbandonedCount > 0 })
				.Select(x => new { x.Key.Abandoned, Count = x.Count() })
				.ToDictionaryAsync(x => x.Abandoned, x => x.Count);

			var neverAbandoned = abandonedGroups.GetValueOrDefault(false);
			var wasAbandoned = abandonedGroups.GetValueOrDefault(true);
			var allTickets = neverAbandoned + wasAbandoned;

			return allTickets > 0 ? neverAbandoned * 100 / allTickets : (int?)null;
		}

		/// <inheritdoc/>
		public async Task<int?> GetFirstAttemptSuccessRateForAll(DateTime? from = null, DateTime? to = null) => await GetFirstAttemptSuccessRate(GetQueryableForAll(), from, to);

		/// <inheritdoc/>
		public async Task<int?> GetFirstAttemptSuccessRateForBrand(Guid brandId, DateTime? from = null, DateTime? to = null) => await GetFirstAttemptSuccessRate(GetQueryableForBrand(brandId), from, to);

		#endregion

		#region GetAverageComplexity

		private async Task<decimal?> GetAverageComplexity(IQueryable<Ticket> queryable, DateTime? from = null, DateTime? to = null)
		{
			queryable = queryable
						.Where(x => x.Status == TicketStatusEnum.Closed);

			queryable = GetTicketsInDateRange(queryable, from, to);

			return await queryable
				.AverageAsync(x => (decimal?)x.Complexity);
		}

		/// <inheritdoc/>
		public async Task<decimal?> GetAverageComplexityForAll(DateTime? from = null, DateTime? to = null) => await GetAverageComplexity(GetQueryableForAll(), from, to);

		/// <inheritdoc/>
		public async Task<decimal?> GetAverageComplexityForBrand(Guid brandId, DateTime? from = null, DateTime? to = null) => await GetAverageComplexity(GetQueryableForBrand(brandId), from, to);

		#endregion

		#region GetAverageTimeToComplete

		/// <inheritdoc/>
		public async Task<double?> GetAverageTimeToComplete(Guid? advocateId = null, Guid? brandId = null, DateTime? from = null, DateTime? to = null)
		{
			if (advocateId == null)
			{
				// average time to complete for the ticket should measure time between first assignment of the ticket and closing
				return (await GetQueryable(brandId: brandId, from: from, to: to)
					.Where(x => x.Status == TicketStatusEnum.Closed)
					.Where(x => x.FirstAssignedDate.HasValue && x.ClosedDate.HasValue)
					.Where(x => x.FraudStatus != FraudStatus.FraudConfirmed)
					.Select(x => x.ClosedDate - x.FirstAssignedDate)
					.Select(x => x.Value.TotalSeconds)
					.ToListAsync()) // as EF3.1 / Pomelo can't properly calculate Average on SQL side, as a workaround we fetch the results and calculate it locally
					.Select(x => (double?)(x))
					.DefaultIfEmpty(null)
					.Average();
			}
			else
			{
				// average time to complete for an advocate should just measure only time that this particular advocate spent on the ticket
				return (await GetQueryable(advocateId: advocateId, brandId: brandId, from: from, to: to)
					.Where(x => x.Status == TicketStatusEnum.Closed)
					.Where(x => x.AssignedDate.HasValue && x.ClosedDate.HasValue)
					.Where(x => x.FraudStatus != FraudStatus.FraudConfirmed)
					.Select(x => x.ClosedDate - x.AssignedDate)
					.Select(x => x.Value.TotalSeconds)
					.ToListAsync()) // as EF3.1 / Pomelo can't properly calculate Average on SQL side, as a workaround we fetch the results and calculate it locally
					.Select(x => (double?)(x))
					.DefaultIfEmpty(null)
					.Average();
			}
		}

		#endregion

		#region GetSuccessRate

		private int? GetSuccessRate(int closedTickets, int everAssignedTickets) => everAssignedTickets > 0 ? closedTickets * 100 / everAssignedTickets : (int?)null;

		private async Task<int> GetClosedTicketsCount(IQueryable<Ticket> queryable)
		{
			return await queryable
				.Where(x => x.Status == TicketStatusEnum.Closed)
				.CountAsync();
		}

		private async Task<int> GetEverAssignedTicketsCount(IQueryable<Ticket> queryable)
		{
			return await queryable
				.Where(x => x.FirstAssignedDate != null)
				.CountAsync();
		}

		private async Task<int> GetEverAssignedToAdvocateTicketsCount(IQueryable<Ticket> queryable, Guid advocateId)
		{
			return await queryable
				.SelectMany(
					t => t.StatusHistory
							.Where(x => x.Status == TicketStatusEnum.Assigned)
							.Where(th => th.AdvocateId == advocateId)
				) //then select just the ticket history fields, from within the inner join subquery
				.Select(x => x.TicketId) //then from that select just the ticketIds.
				.Distinct()
				.CountAsync();
		}

		/// <inheritdoc/>
		public async Task<int?> GetSuccessRate(Guid? advocateId = null, Guid? brandId = null, DateTime? from = null, DateTime? to = null)
		{
			if (advocateId == null)
			{
				var query = GetQueryable(brandId: brandId, from: from, to: to)
						.Where(x => x.FraudStatus != FraudStatus.FraudConfirmed);
				var closedTickets = await GetClosedTicketsCount(query);
				var everAssignedTickets = await GetEverAssignedTicketsCount(query);
				return GetSuccessRate(closedTickets, everAssignedTickets);
			}
			else
			{
				var closedTickets = await GetClosedTicketsCount(GetQueryable(advocateId: advocateId, brandId: brandId, from: from, to: to)
						.Where(x => x.FraudStatus != FraudStatus.FraudConfirmed));
				var everAssignedTickets = await GetEverAssignedToAdvocateTicketsCount(GetQueryable(brandId: brandId, from: from, to: to)
						.Where(x => x.FraudStatus != FraudStatus.FraudConfirmed), advocateId.Value);
				return GetSuccessRate(closedTickets, everAssignedTickets);
			}
		}

		#endregion

		#region GetAverageCsat

		/// <inheritdoc/>
		public async Task<decimal?> GetAverageCsat(Guid? advocateId = null, Guid? brandId = null, DateTime? from = null, DateTime? to = null)
		{
			return await GetQueryable(advocateId: advocateId, brandId: brandId, from: from, to: to)
				.Where(x => x.Status == TicketStatusEnum.Closed)
				.Where(t => t.Csat != null)
				.Where(x => x.FraudStatus != FraudStatus.FraudConfirmed)
				.AverageAsync(x => (decimal?)x.Csat.Value);
		}

		#endregion

		#region GetTotalPrice

		private async Task<decimal> GetTotalPrice(IQueryable<Ticket> queryable, bool includeFees, DateTime? from = null, DateTime? to = null)
		{
			queryable = queryable.Where(x => x.Status == TicketStatusEnum.Closed);
			queryable = GetTicketsInDateRange(queryable, from, to);

			return await queryable.SumAsync(x => includeFees ? x.Price + x.Fee : x.Price);
		}

		/// <inheritdoc/>
		public async Task<decimal> GetTotalPriceForAll(DateTime? from = null, DateTime? to = null) => await GetTotalPrice(GetQueryableForAll(), true, from, to);

		/// <inheritdoc/>
		public async Task<decimal> GetTotalPriceForBrand(Guid brandId) => await GetTotalPrice(GetQueryableForBrand(brandId), true);

		/// <inheritdoc/>
		public async Task<decimal> GetTotalPriceForBrand(Guid brandId, DateTime fromDate, DateTime toDate)
		{
			var queryable = GetQueryableForBrand(brandId)
				.Where(x => x.ClosedDate >= fromDate && x.ClosedDate <= toDate);
			return await GetTotalPrice(queryable, true);
		}

		/// <inheritdoc/>
		public async Task<decimal> GetTotalPriceForAdvocate(Guid advocateId, DateTime? from = null, DateTime? to = null) => await GetTotalPrice(GetQueryableForAdvocate(advocateId), false, from, to);

		#endregion

		#region GetAveragePrice

		private async Task<decimal?> GetAveragePrice(IQueryable<Ticket> queryable, bool includeFees, DateTime? from = null, DateTime? to = null)
		{
			queryable = queryable
						.Where(x => x.Status == TicketStatusEnum.Closed);

			queryable = GetTicketsInDateRange(queryable, from, to);

			return await queryable
						.AverageAsync(x => includeFees ? (decimal?)(x.Price + x.Fee) : (decimal?)x.Price);
		}

		/// <inheritdoc/>
		public async Task<decimal?> GetAveragePriceForAll(DateTime? from = null, DateTime? to = null) => await GetAveragePrice(GetQueryableForAll(), true, from, to);

		/// <inheritdoc/>
		public async Task<decimal?> GetAveragePriceForBrand(Guid brandId, DateTime? from = null, DateTime? to = null) => await GetAveragePrice(GetQueryableForBrand(brandId), true, from, to);

		/// <inheritdoc/>
		public async Task<decimal?> GetAveragePriceForAdvocate(Guid advocateId, DateTime? from = null, DateTime? to = null) => await GetAveragePrice(GetQueryableForAdvocate(advocateId), false, from, to);

		#endregion

		#region GetCountByStatus

		private async Task<Dictionary<TicketStatusEnum, int>> GetCountByStatus(IQueryable<Ticket> queryable)
		{
			return await queryable
				.Where(x => x.IsPractice == false)
				.GroupBy(x => x.Status)
				.Select(x => new { x.Key, Count = x.Count() })
				.ToDictionaryAsync(x => x.Key, x => x.Count);
		}

		/// <inheritdoc/>
		public async Task<Dictionary<TicketStatusEnum, int>> GetCountByStatusForAll() => await GetCountByStatus(GetQueryableForAll());

		/// <inheritdoc/>
		public async Task<Dictionary<TicketStatusEnum, int>> GetCountByStatusForBrand(Guid brandId) => await GetCountByStatus(GetQueryableForBrand(brandId));

		/// <inheritdoc/>
		public async Task<Dictionary<TicketStatusEnum, int>> GetCountByStatusForAdvocate(Guid advocateId) => await GetCountByStatus(GetQueryableForAdvocate(advocateId));

		#endregion

		/// <inheritdoc />
		public async Task<int> GetCriticalCountForBrand(Guid brandId)
		{
			var timestamp = _timestampService.GetUtcTimestamp();
			return await Queryable()
				.Where(x => x.BrandId == brandId)
				.Where(x => x.Status == TicketStatusEnum.New)
				.Where(x =>
					x.RejectionCount >= Ticket.REJECT_CRTICIAL_COUNT ||
					x.AbandonedCount >= Ticket.ABANDON_CRTICIAL_COUNT ||
					timestamp - x.ModifiedDate >= Ticket.CriticalTicketTimeThreshold
				)
				.CountAsync();
		}

		protected IQueryable<Ticket> GetSolverTickets(Guid advocateId)
		{
			var allowedStatus = new TicketStatusEnum[]
			{
				TicketStatusEnum.Assigned, TicketStatusEnum.Solved, TicketStatusEnum.Closed
			};

			var timestamp = _timestampService.GetUtcTimestamp();
			return Queryable()
				.Where(t => t.AdvocateId == advocateId)
				.Where(t =>
					(allowedStatus.Contains(t.Status) && t.Complexity == null) || // ticket is not totaly closed
					(t.Status == TicketStatusEnum.Reserved && t.ReservationExpiryDate > timestamp) // ticket reserved and not expired
				);
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<Ticket>> GetAdvocateTickets(Guid advocateId)
		{
			var query = GetSolverTickets(advocateId);
			var tickets = await query
				.Include(i => i.Customer)
				.Include(i => i.Advocate).ThenInclude(i => i.User)
				.Include(i => i.Brand)
				.OrderBy(t => t.Status)
				.ThenByDescending(t => t.ModifiedDate)
				.ToListAsync();

			await query.Include(i => i.StatusHistory).ThenInclude(i => i.Advocate).ThenInclude(i => i.User).LoadAsync();
			await query.Include(i => i.Tags).ThenInclude(i => i.Tag).LoadAsync();
			await query.Include(i => i.TicketCategory).ThenInclude(i => i.Category).LoadAsync();
			await query.Include(i => i.Metadata).LoadAsync();
			await query.Include(i => i.ProbingAnswers).ThenInclude(i => i.ProbingQuestion).LoadAsync();
			await query.Include(i => i.ProbingAnswers).ThenInclude(i => i.ProbingQuestionOption).LoadAsync();

			return tickets;
		}


		/// <inheritdoc/>
		public async Task<Guid?> GetReservedTicketId(Guid advocateId)
		{
			var timestamp = _timestampService.GetUtcTimestamp();
			return await Queryable()
				.Where(x => x.Status == TicketStatusEnum.Reserved)
				.Where(x => x.ReservationExpiryDate > timestamp)
				.Where(x => x.AdvocateId == advocateId)
				.Select(x => (Guid?)x.Id)
				.SingleOrDefaultAsync();
		}

		/// <inheritdoc/>
		public async Task<Ticket> GetFullTicket(Expression<Func<Ticket, bool>> predicate = null)
		{
			var query = Queryable();
			query = predicate != null ? query.Where(predicate) : query;
			var ticket = await query
				.Include(inc => inc.Source)
				.Include(inc => inc.Customer)
				.Include(inc => inc.Brand).ThenInclude(brnd => brnd.Tags)
				.Include(inc => inc.Tags)
				.Include(inc => inc.TicketCategory)
				.Include(inc => inc.Advocate).ThenInclude(tin => tin.User)
				.SingleOrDefaultAsync();

			if (ticket != null)
			{
				await query.Include(inc => inc.Advocate).ThenInclude(tin => tin.Brands).LoadAsync();
				await query.Include(inc => inc.Tags).ThenInclude(tin => tin.Tag).LoadAsync();
				await query.Include(inc => inc.TicketCategory).ThenInclude(tin => tin.Category).LoadAsync();
				await query.Include(inc => inc.Metadata).LoadAsync();
				await query.Include(inc => inc.ProbingAnswers).ThenInclude(tin => tin.ProbingQuestion).LoadAsync();
				await query.Include(inc => inc.ProbingAnswers).ThenInclude(tin => tin.ProbingQuestionOption).LoadAsync();
				await query.Include(inc => inc.StatusHistory).ThenInclude(tin => tin.Advocate).ThenInclude(tin => tin.User).LoadAsync();
				await query.Include(inc => inc.RejectionHistory).LoadAsync();
				await query.Include(inc => inc.AbandonHistory).LoadAsync();
				await query.Include(inc => inc.TrackingHistory).LoadAsync();
			}

			return ticket;
		}

		/// <inheritdoc/>
		public async Task<Ticket> GetTicketWithTaggingInfo(Expression<Func<Ticket, bool>> predicate = null)
		{
			var query = Queryable();
			query = predicate != null ? query.Where(predicate) : query;
			var ticket = await query
				.Include(inc => inc.Brand).ThenInclude(brnd => brnd.Tags)
				.Include(inc => inc.Advocate).ThenInclude(tin => tin.User)
				.SingleOrDefaultAsync();

			if (ticket != null)
			{
				await query.Include(inc => inc.Tags).ThenInclude(tin => tin.Tag).LoadAsync();
				await query.Include(inc => inc.TicketCategory).ThenInclude(tin => tin.Category).LoadAsync();
			}

			return ticket;
		}

		/// <inheritdoc/>
		public async Task<Guid?> GetPracticeTicket(Guid advocateId) => await GetSingleOrDefaultAsync(x => x.Id, x => x.AdvocateId == advocateId && x.IsPractice);

		/// <inheritdoc/>
		public async Task<IEnumerable<RejectionReason>> GetRejectionReasons() =>
			await DbContext.Set<RejectionReason>()
			.Where(x => x.Id != RejectionReason.ReservationExpiredReasonId)
			.ToListAsync();

		/// <inheritdoc/>
		public async Task<bool> RejectReasonsExist(int[] rejectReasonIds) => await DbContext.Set<RejectionReason>().CountAsync(rr => rejectReasonIds.Contains(rr.Id)) == rejectReasonIds.Count();

		/// <inheritdoc/>
		public async Task<IList<Ticket>> GetExpiredTickets()
		{
			return await Queryable()
				.Where(x => x.Status == TicketStatusEnum.Reserved)
				.Where(x => x.ReservationExpiryDate < DateTime.UtcNow)
				.Include(x => x.RejectionHistory)
				.Include(x => x.StatusHistory)
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<bool> ReserveTicket(Guid advocateId, IEnumerable<Guid> advocateBrandIds, TicketLevel level)
		{
			var preliminaryExpirationDate =
				_timestampService.GetUtcTimestamp().AddMinutes(Ticket.NormalReserveExpiryMin);
			var brands = advocateBrandIds.ToList();

			var policy = Policy.Handle<DbUpdateConcurrencyException>().Or<DbUpdateException>().RetryAsync(3,
				onRetry: (exception, retryCount) =>
				{
					_logger.LogDebug("Hit an exception {0}, and now retrying #{1}", exception, retryCount);
				});

			var affectedRows = 0;

			await policy.ExecuteAsync(async () =>
			{
				affectedRows = await DbSet
					.Where(x => x.IsPractice == false && x.Ready)
					.Where(x => x.Status == TicketStatusEnum.New) // look only for tickets in new state
					.Where(x => x.Level == level) // look only for tickets in required queue
					.Where(x => x.AbandonHistory.All(ah =>
						ah.AdvocateId != advocateId ||
						ah.Reasons.All(r =>
							r.AbandonReason
								.IsAutoAbandoned))) // the ticket could not have been abandoned by advocate in the past (unless it was AutoAbandonment)
					.Where(x => brands.Contains(x
						.BrandId)) // Filter only the eligible brands for this advocate.
					.OrderByDescending(t => t.AbandonedCount)
					.ThenBy(t => t.CreatedDate) // get the most urgent ticket
					.Take(1)
					.UpdateAsync(x => new Ticket
					{
						Status = TicketStatusEnum.Reserved,
						AdvocateId = advocateId,
						ReservationExpiryDate = preliminaryExpirationDate,
					}, x =>
					{
						x.UseRowLock = true;
					});
			});

			return affectedRows > 0;
		}

		private IQueryable<Ticket> GetQueryable(Guid? advocateId = null, Guid? brandId = null, DateTime? from = null, DateTime? to = null)
		{
			var query = Queryable();
			query = GetTicketsInDateRange(query, from, to);
			query = advocateId != null ? query.Where(x => x.AdvocateId == advocateId.Value) : query;
			query = brandId != null ? query.Where(x => x.BrandId == brandId.Value) : query;
			return query;
		}

		private IQueryable<Ticket> GetQueryableForAll() => Queryable();

		private IQueryable<Ticket> GetQueryableForBrand(Guid brandId) => Queryable().Where(x => x.BrandId == brandId);

		private IQueryable<Ticket> GetQueryableForAdvocate(Guid advocateId) => Queryable().Where(x => x.AdvocateId == advocateId && x.FraudStatus != FraudStatus.FraudConfirmed && x.TagStatus != null);

		/// <inheritdoc/>
		public async Task<(decimal priceTotal, decimal feeTotal, int ticketsCount)> GetClientInvoicingAmounts(
			DateTime fromDate, DateTime toDate, Guid brandId)
		{
			var result = await QueryTicketsForInvoicing(Queryable(), fromDate, toDate, QueryInvoicingTarget.Client)
				.Where(t => t.BrandId == brandId)
				.GroupBy(x => x.BrandId)
				.Select(x => new
				{
					priceTotal = x.Sum(s => s.Price),
					feeTotal = x.Sum(s => s.Fee),
					ticketsCount = x.Count()
				})
				.SingleOrDefaultAsync();

			return result != null ? (result.priceTotal, result.feeTotal, result.ticketsCount) : (0m, 0m, 0);
		}

		/// <inheritdoc/>
		public Task<int> SetTicketsClientInvoiceId(Guid invoiceId, Guid brandId, DateTime fromDate, DateTime toDate)
		{
			return QueryTicketsForInvoicing(DbSet, fromDate, toDate, QueryInvoicingTarget.Client)
				.Where(t => t.BrandId == brandId)
				.UpdateAsync(x => new Ticket() { ClientInvoiceId = invoiceId }, acquireLock: true);
		}

		/// <inheritdoc/>
		public Task<int> SetTicketsAdvocateInvoiceId(Guid invoiceId, Guid advocateId, DateTime fromDate, DateTime toDate)
		{
			return QueryTicketsForInvoicing(DbSet, fromDate, toDate, QueryInvoicingTarget.Advocate)
				.Where(t => t.AdvocateId == advocateId)
				.UpdateAsync(x => new Ticket() { AdvocateInvoiceId = invoiceId }, acquireLock: true);
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<Guid>> GetAdvocatesToInvoice(DateTime fromDate, DateTime toDate)
		{
			return await QueryTicketsForInvoicing(Queryable(), fromDate, toDate, QueryInvoicingTarget.Advocate)
				.Select(x => x.AdvocateId.Value)
				.Distinct()
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<List<(Guid brandId, decimal priceTotal, int ticketsCount)>> GetTicketsForAdvocateInvoice(DateTime fromDate, DateTime toDate, Guid advocateId)
		{
			var result = await QueryTicketsForInvoicing(Queryable(), fromDate, toDate, QueryInvoicingTarget.Advocate)
				.Where(x => x.AdvocateId == advocateId)
				.GroupBy(x => new { x.BrandId, x.AdvocateId })
				.Select(x => new
				{
					brandId = x.Key.BrandId,
					priceTotal = x.Sum(s => s.Price),
					ticketsCount = x.Count()
				})
				.ToListAsync();

			return result
				.Select(x => (x.brandId, x.priceTotal, x.ticketsCount))
				.ToList();
		}

		private IQueryable<Ticket> QueryTicketsForInvoicing(IQueryable<Ticket> queryable, DateTime fromDate,
			DateTime toDate, QueryInvoicingTarget target)
		{
			return queryable
				.Where(t => t.ClosedDate >= fromDate && t.ClosedDate < toDate)
				.Where(t => (target == QueryInvoicingTarget.Advocate ? t.AdvocateInvoiceId == null : t.ClientInvoiceId == null))
				.Where(x => x.FraudStatus != FraudStatus.FraudConfirmed)
				.Where(x => x.AdvocateId != null)
				.Where(x => x.IsPractice == false)
				.Where(x => x.Level == TicketLevel.Regular);
		}

		/// <inheritdoc/>
		public Task<List<(Guid id, Guid brandId, decimal price)>> GetTicketsInfoForAdvocateInvoice(Guid advocateInvoiceId)
		{
			var query = from ticket in Queryable()
						where ticket.AdvocateInvoiceId == advocateInvoiceId
						select Tuple.Create(ticket.Id, ticket.BrandId, ticket.Price).ToValueTuple();

			return query.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<IList<(string source, int count)>> GetEscalatedBySource(Guid brandId, DateTime? from, DateTime? to)
		{
			var result = Queryable()
				.Where(x => x.BrandId == brandId)
				.Where(x => x.Status == TicketStatusEnum.Escalated);

			result = GetTicketsInDateRange(result, from, to);


			var formattedResult = await result
										.GroupBy(x => new { x.BrandId, x.Source.Name })
										.Select(x => new { x.Key.Name, Count = x.Count() })
										.ToListAsync();

			return formattedResult
				.Select(x => (x.Name, x.Count))
				.ToList();
		}

		/// <inheritdoc/>
		public async Task<Guid?> GetBrandId(Guid ticketId) => await GetFirstOrDefaultAsync(x => x.BrandId, x => x.Id == ticketId);

		/// <inheritdoc/>
		public async Task<(int closedTickets, decimal amount)> GetClosedStatisticForPeriod(Guid advocateId, DateTimeOffset? from, DateTime? to)
		{
			var query = Queryable()
				.Where(x => x.Status == TicketStatusEnum.Closed && x.TagStatus != null)
				.Where(x => x.IsPractice == false)
				.Where(x => x.AdvocateId == advocateId)
				.Where(x => x.FraudStatus != FraudStatus.FraudConfirmed);

			if (from != null)
			{
				query = query.Where(x => x.ClosedDate >= from);
			}

			if (to != null)
			{
				query = query.Where(x => x.ClosedDate < to);
			}


			var result = await query
				.GroupBy(x => x.AdvocateId)
				.Select(x => new
				{
					Amount = x.Sum(t => t.Price),
					Count = x.Count()
				})
				.FirstOrDefaultAsync();

			return (result?.Count ?? 0, result?.Amount ?? 0m);
		}

		/// <inheritdoc/>
		public async Task<IPagedList<Ticket>> GetPagedExportData(DateTime? from, DateTime? to, Guid? brandId, int pageIndex, int pageSize = 100)
		{
			var fromTimestamp = from ?? DateTime.MinValue;
			var toTimestamp = to ?? DateTime.MaxValue;
			var query = Queryable()
				.Where(x => x.IsPractice == false)
				.Where(x => (x.CreatedDate >= fromTimestamp && x.CreatedDate <= toTimestamp) || (x.ModifiedDate >= fromTimestamp && x.ModifiedDate <= toTimestamp));

			if (brandId.HasValue)
			{
				query = query.Where(t => t.BrandId == brandId);
			}

			var orderedQuery = query.OrderBy(o => o.CreatedDate);

			var page = await orderedQuery.ToPagedListAsync(pageIndex, pageSize);

			await orderedQuery.Include(x => x.StatusHistory).ThenInclude(x => x.Advocate).ThenInclude(x => x.User).LoadPagedAsync(pageIndex, pageSize);
			await orderedQuery.Include(x => x.RejectionHistory).ThenInclude(x => x.Reasons).ThenInclude(x => x.RejectionReason).LoadPagedAsync(pageIndex, pageSize);
			await orderedQuery.Include(x => x.Brand).ThenInclude(x => x.Tags).LoadPagedAsync(pageIndex, pageSize);
			await orderedQuery.Include(x => x.Metadata).LoadPagedAsync(pageIndex, pageSize);
			await orderedQuery.Include(x => x.Advocate).ThenInclude(x => x.User).LoadPagedAsync(pageIndex, pageSize);
			await orderedQuery.Include(x => x.AbandonHistory).ThenInclude(x => x.Reasons).ThenInclude(x => x.AbandonReason).LoadPagedAsync(pageIndex, pageSize);
			await orderedQuery.Include(x => x.Source).LoadPagedAsync(pageIndex, pageSize);
			await orderedQuery.Include(x => x.Tags).LoadPagedAsync(pageIndex, pageSize);
			await orderedQuery.Include(x => x.EscalatedSolver).ThenInclude(x => x.User).LoadPagedAsync(pageIndex, pageSize);
			await orderedQuery.Include(x => x.ProbingAnswers).ThenInclude(x => x.ProbingQuestion).LoadPagedAsync(pageIndex, pageSize);
			await orderedQuery.Include(x => x.ProbingAnswers).ThenInclude(x => x.ProbingQuestionOption).LoadPagedAsync(pageIndex, pageSize);
			await orderedQuery.Include(x => x.TicketCategory).ThenInclude(x => x.Category).LoadPagedAsync(pageIndex, pageSize);

			return page;
		}

		/// <summary>
		/// Applies a where clause against the ticket repository, for a given date range, and an optioonal IsPractice clause
		/// </summary>
		/// <param name="query">The Queryable Ticket repository, to apply the Where clauses against</param>
		/// <param name="from">Starting DateTime to apply to the start date clause</param>
		/// <param name="to">Ending DateTime to apply to the end date clause</param>
		/// <param name="isPractice">Defaults to false if not passed in</param>
		/// <returns>An IQueryable of Ticket, with the applied predicates</returns>
		private IQueryable<Ticket> GetTicketsInDateRange(IQueryable<Ticket> query, DateTime? from, DateTime? to, bool isPractice = false)
		{
			var fromTimestamp = from ?? DateTime.MinValue;
			var toTimestamp = to ?? DateTime.MaxValue;

			query = query
					.Where(t => t.IsPractice == isPractice)
					.Where(x =>
						x.Status == TicketStatusEnum.Escalated ? x.EscalatedDate >= fromTimestamp && x.EscalatedDate <= toTimestamp : // for escalated tickets, range in EscalatedDate
						x.Status == TicketStatusEnum.Closed ? x.ClosedDate >= fromTimestamp && x.ClosedDate <= toTimestamp : // for closed tickets, range in ClosedDate
						x.CreatedDate >= fromTimestamp && x.CreatedDate <= toTimestamp // for all other, range in CreatedDate
					);
			return query;
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<Guid>> GetAvailableTicketIds(Guid brandId, TicketLevel level)
		{
			return await Queryable()
				.Where(t => t.Status == TicketStatusEnum.New && t.Ready)
				.Where(t => t.IsPractice == false)
				.Where(t => t.Level == level)
				.Where(t => t.BrandId == brandId)
				.Select(t => t.Id)
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<Guid>> GetTouchedTicketIds(Guid advocateId)
		{
			return await Queryable()
				.Where(t => t.Status != TicketStatusEnum.Closed && t.Ready)
				.Where(t => t.IsPractice == false)
				.Where(t => t.Level < TicketLevel.PushedBack)
				.Where(x => x.AbandonHistory.Any(ah => ah.AdvocateId == advocateId && ah.Reasons.All(r => r.AbandonReason.IsAutoAbandoned == false))) // the ticket was already abandoned (but not by AutoAbandonment)
				.Select(t => t.Id)
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<Tag>> GetTags(Guid ticketId, TicketLevel userLevel)
		{
			return await Queryable()
				.Where(x => x.Id == ticketId)
				.SelectMany(x => x.Tags.Where(t => t.Level == userLevel).Select(y => y.Tag))
				.ToListAsync();
		}

		/// <inheritdoc/>
		public Task<List<Guid>> GetEscalatedTicketIds(Guid brandId) =>
			Queryable()
			.Where(t => t.Status == TicketStatusEnum.Escalated)
			.Where(t => !t.IsPractice) // Currently no practice ticket can be escalated, but who knows 🤷‍
			.Where(t => t.BrandId == brandId)
			.Select(t => t.Id).ToListAsync();

		/// <inheritdoc/>
		public async Task<IList<(DateTime key, Guid brandId, decimal totalPrice)>> GetAdvocatePerformanceBreakDownForPeriod(Guid advocateId, DateTime from, DateTime to, Guid[] brandIds, Period period)
		{
			var query = from ticket in Queryable()
						join advocateBrand in DbContext.Set<AdvocateBrand>()
						on new { AdvocateId = ticket.AdvocateId.Value, BrandId = ticket.BrandId } equals new { AdvocateId = advocateBrand.AdvocateId, BrandId = advocateBrand.BrandId }
						where ticket.Status == TicketStatusEnum.Closed &&
						ticket.IsPractice == false &&
						ticket.AdvocateId == advocateId &&
						advocateBrand.Authorized == true &&
						ticket.FraudStatus != FraudStatus.FraudConfirmed
						select new
						{
							BrandId = ticket.BrandId,
							ClosedDate = ticket.ClosedDate,
							Price = ticket.Price
						};

			if (brandIds != null && brandIds.Any())
			{
				query = query.Where(x => brandIds.Contains(x.BrandId));
			}

			query = query.Where(x => x.ClosedDate >= @from);

			query = query.Where(x => x.ClosedDate < to);

			var result = period == Period.Year ? await query
				.GroupBy(x => new { x.ClosedDate.Value.Month, x.BrandId })
				.Select(x => new
				{
					Date = new DateTime(from.Year, x.Key.Month, 1),
					BrandId = x.Key.BrandId,
					TotalPrice = x.Sum(x => x.Price)
				}).ToListAsync() : await query
				.GroupBy(x => new { x.ClosedDate.Value.Date, x.BrandId })
				.Select(x => new
				{
					Date = x.Key.Date,
					BrandId = x.Key.BrandId,
					TotalPrice = x.Sum(x => x.Price)
				}).ToListAsync();

			return result.Select(x => (x.Date, x.BrandId, x.TotalPrice)).ToList();
		}

		/// <inheritdoc/>
		public async Task<IList<(Guid brandId, decimal totalPrice, int totalCount)>> GetAdvocatePerformanceSummaryForPeriod(Guid advocateId, DateTime from, DateTime to, Guid[] brandIds)
		{
			var query = from ticket in Queryable()
						join advocateBrand in DbContext.Set<AdvocateBrand>()
						on new { AdvocateId = ticket.AdvocateId.Value, BrandId = ticket.BrandId } equals new { AdvocateId = advocateBrand.AdvocateId, BrandId = advocateBrand.BrandId }
						where ticket.Status == TicketStatusEnum.Closed &&
						ticket.IsPractice == false &&
						ticket.AdvocateId == advocateId &&
						advocateBrand.Authorized == true &&
						ticket.FraudStatus != FraudStatus.FraudConfirmed
						select new
						{
							BrandId = ticket.BrandId,
							ClosedDate = ticket.ClosedDate,
							Price = ticket.Price
						};

			if (brandIds != null && brandIds.Any())
			{
				query = query.Where(x => brandIds.Contains(x.BrandId));
			}

			query = query.Where(x => x.ClosedDate >= @from);

			query = query.Where(x => x.ClosedDate < to);

			var result = await query
				.GroupBy(x => x.BrandId)
				.Select(x => new
				{
					BrandId = x.Key,
					TotalPrice = x.Sum(x => x.Price),
					ClosedTickets = x.Count()
				}).ToListAsync();

			return result.Select(x => (x.BrandId, x.TotalPrice, x.ClosedTickets)).ToList();
		}

		/// <inheritdoc/>
		public async Task<IList<TResult>> GetAssignedTickets<TResult>(Guid advocateId, Expression<Func<Ticket, TResult>> selector)
		{
			return await Queryable()
				.Where(t => t.AdvocateId == advocateId)
				.Where(t => !t.IsPractice)
				.Where(t => t.Status == TicketStatusEnum.Assigned)
				.Select(selector)
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<Guid?> GetReturningCustomerTicketId(Guid brandId, string email, IReadOnlyDictionary<string, string> keyMetadata)
		{
			var query = Queryable()
				.Where(x => x.BrandId == brandId)
				.Where(x => x.Customer.Email == email)
				.Where(x => new[] { TicketStatusEnum.New, TicketStatusEnum.Assigned, TicketStatusEnum.Reserved, TicketStatusEnum.Solved }.Contains(x.Status));

			foreach (var item in keyMetadata)
			{
				query = query.Where(x => x.Metadata.Any(m => m.Key == item.Key && m.Value == item.Value));
			}

			return await query
				.OrderByDescending(x => x.CreatedDate)
				.Select(x => (Guid?)x.Id)
				.FirstOrDefaultAsync();
		}

		/// <inheritdoc/>
		public async Task<string> GetSerialNumber(Guid ticketId)
		{
			return await Queryable()
				.Where(t => t.Id == ticketId)
				.SelectMany(t => t.Metadata)
				.Where(tmi => tmi.Key.Equals(SerialNumber, StringComparison.InvariantCultureIgnoreCase))
				.Select(tmi => tmi.Value).FirstOrDefaultAsync();
		}

		/// <inheritdoc/>
		public async Task<DateTime?> GetLastDateForTicketStatus(Guid ticketId, Guid advocateId, TicketStatusEnum status)
		{
			return await Queryable()
				.Where(t => t.Id == ticketId)
				.SelectMany(t => t.StatusHistory)
				.OrderBy(sh => sh.CreatedDate)
				.Where(sh => sh.AdvocateId.Value == advocateId && sh.Status == status)
				.Select(sh => sh.CreatedDate).LastOrDefaultAsync();
		}

		/// <inheritdoc/>
		public async Task<Category> GetCategory(Guid ticketId) => await GetSingleOrDefaultAsync(predicate: t => t.Id == ticketId, selector: t => t.TicketCategory.Category);

		/// <inheritdoc/>
		public async Task<IEnumerable<Ticket>> GetEscalatedDiagnosisEnabledTicket(Expression<Func<Ticket, bool>> predicate)
		{
			var query = Queryable()
				.Where(predicate);

			var allowedStatus = new TicketStatusEnum[]
			{
				TicketStatusEnum.New, TicketStatusEnum.Reserved, TicketStatusEnum.Assigned, TicketStatusEnum.Solved
			};

			var tickets = await query
				.Where(t => t.Level == TicketLevel.SuperSolver)
				.Where(t => allowedStatus.Contains(t.Status) || (t.Status == TicketStatusEnum.Closed && t.TagStatus == null))
				.Include(i => i.Customer)
				.Include(i => i.Advocate).ThenInclude(i => i.User)
				.Include(i => i.Brand)
				.OrderBy(t => t.Status)
				.ThenByDescending(t => t.ModifiedDate)
				.ToListAsync();

			await query.Include(i => i.StatusHistory).ThenInclude(i => i.Advocate).ThenInclude(i => i.User).LoadAsync();
			await query.Include(i => i.Tags).ThenInclude(i => i.Tag).LoadAsync();
			await query.Include(i => i.TicketCategory).ThenInclude(i => i.Category).LoadAsync();
			await query.Include(i => i.Metadata).LoadAsync();
			await query.Include(i => i.ProbingAnswers).ThenInclude(i => i.ProbingQuestion).LoadAsync();
			await query.Include(i => i.ProbingAnswers).ThenInclude(i => i.ProbingQuestionOption).LoadAsync();

			return tickets;
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<ProbingResult>> GetProbingResults(Guid ticketId)
		{
			return await Queryable()
				.Where(x => x.Id == ticketId)
				.Include(x => x.ProbingAnswers)
				.ThenInclude(x => x.ProbingQuestionOption)
				.Select(x => x.ProbingAnswers)
				.FirstOrDefaultAsync();
		}

		/// <inheritdoc/>
		public async Task<List<Ticket>> GetCustomerTickets(Guid customerId)
		{
			return await Queryable().Include(x => x.Brand).Include(x => x.Metadata)
			.Where(x => x.Customer.Id == customerId
			&& (x.Status == TicketStatusEnum.Assigned || x.Status == TicketStatusEnum.New)
			&& x.Brand.EnableCustomerEndpoint == true).OrderByDescending(x => x.ModifiedDate).Take(10).ToListAsync();
		}

		/// <inheritdoc/>
		public async Task SetTicketReady(Guid ticketId) => await DbSet.Where(t => t.Id == ticketId).UpdateAsync(x => new Ticket { Ready = true });

		public async Task<List<(Guid brandId, string brandName, decimal priceTotal, int ticketsCount)>> GetTicketsWithBrandNameForAdvocateInvoice(DateTime fromDate, DateTime toDate, Guid advocateId)
		{
			var result = await QueryTicketsForInvoicing(Queryable(), fromDate, toDate, QueryInvoicingTarget.Advocate)
			   .Where(x => x.AdvocateId == advocateId)
			   .Include(x => x.Brand)
			   .GroupBy(x => new { x.BrandId, x.Brand.Name, x.AdvocateId })
			   .Select(x => new
			   {
				   brandId = x.Key.BrandId,
				   brandName = x.Key.Name,
				   priceTotal = x.Sum(s => s.Price),
				   ticketsCount = x.Count()
			   })
				.ToListAsync();

			return result
				.Select(x => (x.brandId, x.brandName, x.priceTotal, x.ticketsCount))
				.ToList();
		}
	}

	internal enum QueryInvoicingTarget
	{
		Advocate = 0,
		Client = 1
	}
}