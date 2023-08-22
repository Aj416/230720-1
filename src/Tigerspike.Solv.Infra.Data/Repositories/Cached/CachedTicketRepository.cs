using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ServiceStack.Redis;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Redis;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Infra.Data.Models;
using static Tigerspike.Solv.Core.Constants.CacheKeys;

namespace Tigerspike.Solv.Infra.Data.Repositories.Cached
{
	public class CachedTicketRepository : ICachedTicketRepository
	{
		private const int MonthsInYear = 12;
		private readonly IMapper _mapper;
		private readonly ITicketRepository _ticketRepository;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly ITimestampService _timestampService;

		public CachedTicketRepository(
			IMapper mapper,
			ITicketRepository ticketRepository,
			IRedisClientsManager redisClientsManager,
			ITimestampService timestampService,
			IPaymentRepository paymentRepository)
		{
			_mapper = mapper;
			_ticketRepository = ticketRepository;
			_redisClientsManager = redisClientsManager;
			_timestampService = timestampService;
			_paymentRepository = paymentRepository;
		}

		/// <inheritdoc/>
		public async Task<TicketTransportType> GetTransportType(Guid ticketId)
		{
			using var client = _redisClientsManager.GetClient();
			var key = GetTicketTransportTypeKey(ticketId);

			if (client.ContainsKey(key))
			{
				return client.Get<TicketTransportType>(key);
			}

			var value = await _ticketRepository.GetSingleOrDefaultAsync(x => x.TransportType, x => x.Id == ticketId);
			return client.Set(key, value, expireIn: TimeSpan.FromDays(30));
		}

		/// <inheritdoc/>
		public async Task<ReturningCustomerFlowContext> GetReturningCustomerFlowContext(Guid ticketId)
		{
			using var connection = _redisClientsManager.GetClient();
			var typedClient = connection.As<ReturningCustomerFlowContext>();
			var key = GetReturningCustomerFlowContextKey(ticketId);

			if (typedClient.ContainsKey(key) == false)
			{
				var value = await _ticketRepository.GetFirstOrDefaultAsync<ReturningCustomerFlowContext>(_mapper, x => x.Id == ticketId);
				typedClient.SetValue(key, value, expireIn: TimeSpan.FromDays(7));
			}

			return typedClient.GetValue(key);
		}

		/// <inheritdoc/>
		public void SetReturningCustomerFlowContext(ReturningCustomerFlowContext context)
		{
			using var connection = _redisClientsManager.GetClient();
			var typedClient = connection.As<ReturningCustomerFlowContext>();
			var key = GetReturningCustomerFlowContextKey(context.TicketId);
			typedClient.SetValue(key, context, expireIn: TimeSpan.FromDays(7));
		}

		/// <inheritdoc/>
		public async Task<NotificationResumptionFlowContext> GetNotificationResumptionFlowContext(Guid ticketId)
		{
			using var connection = _redisClientsManager.GetClient();
			var typedClient = connection.As<NotificationResumptionFlowContext>();
			var key = GetNotificationResumptionFlowContextKey(ticketId);

			if (typedClient.ContainsKey(key) == false)
			{
				var value = await _ticketRepository.GetFirstOrDefaultAsync<NotificationResumptionFlowContext>(_mapper, x => x.Id == ticketId);
				typedClient.SetValue(key, value, expireIn: TimeSpan.FromDays(7));
			}

			return typedClient.GetValue(key);
		}

		/// <inheritdoc/>
		public void SetNotificationResumptionFlowContext(NotificationResumptionFlowContext context)
		{
			using var connection = _redisClientsManager.GetClient();
			var typedClient = connection.As<NotificationResumptionFlowContext>();
			var key = GetNotificationResumptionFlowContextKey(context.TicketId);
			typedClient.SetValue(key, context, expireIn: TimeSpan.FromDays(7));
		}

		/// <inheritdoc/>
		public async Task<TicketTransportModel> GetTransportModel(Guid ticketId, Guid? advocateId)
		{
			using var client = _redisClientsManager.GetClient();
			var key = GetTicketTransportModelKey(ticketId, advocateId);

			if (client.ContainsKey(key) == false)
			{
				var value = await _ticketRepository.GetSingleOrDefaultAsync(x => new TicketTransportModel
				{
					Id = x.Id,
					Number = x.Number,
					TransportType = x.TransportType,
					CustomerEmail = x.Customer.Email,
					CustomerFirstName = x.Customer.FirstName,
					AdvocateFirstName = x.Advocate.User.FirstName,
					BrandName = x.Brand.Name,
					BrandLogoUrl = x.Brand.Logo,
					Question = x.Question,
					EndChatEnabled = x.Brand.EndChatEnabled,
					Culture = x.Culture,
					ClosingTime = x.Brand.WaitMinutesToClose,
				}, x => x.Id == ticketId && x.AdvocateId == advocateId);

				if (value != null)
				{
					client.Set(key, value, expireIn: TimeSpan.FromDays(30));
				}
			}

			return client.Get<TicketTransportModel>(key);
		}

		/// <inheritdoc/>
		public async Task<AdvocateStatisticPeriodSummaryModel> UpdateStatistics(Guid advocateId)
		{
			return new AdvocateStatisticPeriodSummaryModel {
				PreviousWeek = await GetPreviousWeekStatisticsPeriod(advocateId),
				CurrentWeek = await SetCurrentWeekStatisticsPeriod(advocateId),
				AllTime = await SetAllTimeStatisticsPeriod(advocateId),
			};
		}

		/// <inheritdoc/>
		public async Task<AdvocateStatisticPeriodSummaryModel> GetStatisticsPeriodPackage(Guid advocateId)
		{
			return new AdvocateStatisticPeriodSummaryModel
			{
				PreviousWeek = await GetPreviousWeekStatisticsPeriod(advocateId),
				CurrentWeek = await GetCurrentWeekStatisticsPeriod(advocateId),
				AllTime = await GetAllTimeStatisticsPeriod(advocateId),
			};
		}

		private async Task<AdvocateStatisticPeriodModel> GetPreviousWeekStatisticsPeriod(Guid advocateId)
		{
			var to = _timestampService.GetUtcTimestamp().StartOfWeek();
			var from = to.AddDays(-7); // last week start
			var key = PreviousWeekStatisticsPeriodKey(advocateId);
			var exp = to.AddDays(7); // expire cache at the end of this week (because by then this data would need to be recalculated)

			using var client = _redisClientsManager.GetClient();

			if (client.ContainsKey(key))
			{
				return client.Get<AdvocateStatisticPeriodModel>(key);
			}

			var statistics = await GetStatisticPeriod(advocateId, from, to);
			var paidOn = await _paymentRepository.GetLastPaymentDate(advocateId);
			statistics.PaidOn = paidOn;

			return client.Set(key, statistics, expireAt: exp);
		}

		private async Task<AdvocateStatisticPeriodModel> SetCurrentWeekStatisticsPeriod(Guid advocateId)
		{
			var sow = _timestampService.GetUtcTimestamp().StartOfWeek();
			var from = sow; // current week start
			var to = from.AddDays(7); // current week end
			var key = CurrentWeekStatisticsPeriodKey(advocateId);
			var exp = sow.AddDays(7); // expire cache at the end of this week (because by then this data would need to be recalculated)

			using var client = _redisClientsManager.GetClient();
			var value = await GetStatisticPeriod(advocateId, from, to);
			return client.Set(key, value, expireAt: exp);
		}

		private async Task<AdvocateStatisticPeriodModel> GetCurrentWeekStatisticsPeriod(Guid advocateId)
		{
			var sow = _timestampService.GetUtcTimestamp().StartOfWeek();
			var from = sow; // current week start
			var to = from.AddDays(7); // current week end
			var key = CurrentWeekStatisticsPeriodKey(advocateId);
			var exp = sow.AddDays(7); // expire cache at the end of this week (because by then this data would need to be recalculated)

			using var client = _redisClientsManager.GetClient();

			if (client.ContainsKey(key))
			{
				return client.Get<AdvocateStatisticPeriodModel>(key);
			}

			var value = await GetStatisticPeriod(advocateId, from, to);
			return client.Set(key, value, expireAt: exp);
		}

		private async Task<AdvocateStatisticPeriodModel> SetAllTimeStatisticsPeriod(Guid advocateId)
		{
			var key = AllTimeStatisticsPeriodKey(advocateId);
			var exp = _timestampService.GetUtcTimestamp().AddDays(7); // expire cache 7 days from now (arbitrary date, no special reason)

			using var client = _redisClientsManager.GetClient();
			var value = await GetStatisticPeriod(advocateId, null, null);

			return client.Set(key, value, expireAt: exp);
		}

		private async Task<AdvocateStatisticPeriodModel> GetAllTimeStatisticsPeriod(Guid advocateId)
		{
			var key = AllTimeStatisticsPeriodKey(advocateId);
			var exp = _timestampService.GetUtcTimestamp().AddDays(7); // expire cache 7 days from now (arbitrary date, no special reason)

			using var client = _redisClientsManager.GetClient();

			if (client.ContainsKey(key))
			{
				return client.Get<AdvocateStatisticPeriodModel>(key);
			}

			var value = await GetStatisticPeriod(advocateId, null, null);

			return client.Set(key, value, expireAt: exp);
		}

		private async Task<AdvocateStatisticPeriodModel> GetStatisticPeriod(Guid advocateId, DateTime? from, DateTime? to)
		{
			var (closedTickets, amount) = await _ticketRepository.GetClosedStatisticForPeriod(advocateId, from, to);
			return new AdvocateStatisticPeriodModel
			{
				Amount = amount,
				ClosedTickets = closedTickets,
				From = from?.Date,
				To = to?.Date.AddDays(-1)
			};
		}

		/// <inheritdoc/>
		public async Task<AdvocatePerformanceStatisticPeriodSummaryModel> GetAdvocatePerformanceStatisticsPeriod(Guid advocateId, DateTime from, DateTime to, Guid[] brandIds, Period period, Guid[] advocateBrandIds)
		{
			return new AdvocatePerformanceStatisticPeriodSummaryModel
			{
				Breakdown = await GetAdvocatePerformanceBreakDown(advocateId, from, to, brandIds, period, advocateBrandIds),
				Summary = await GetAdvocatePerformanceSummary(advocateId, from, to, brandIds, period, advocateBrandIds)
			};
		}

		private async Task<List<AdvocatePerformanceBreakDownModel>> GetAdvocatePerformanceBreakDown(Guid advocateId,
			DateTime from, DateTime to, Guid[] brandIds, Period period, Guid[] advocateBrandIds)
		{
			using var client = _redisClientsManager.GetClient();
			var typedClient = client.As<List<AdvocatePerformanceBreakDownModel>>();

			var brands = brandIds != null && brandIds.Any() ? String.Join("|", brandIds) : "All";
			var key = _timestampService.GetUtcTimestamp().StartOfWeek() >= to
				? AdvocatePerformanceBreakDownKey(advocateId, from, period.ToString(), brands)
				: null;

			if (!string.IsNullOrEmpty(key))
			{
				if (!client.ContainsKey(key))
				{
					var value = await GetAdvocatePerformanceBreakDownForPeriod(advocateId, from, to, brandIds, period, advocateBrandIds);
					typedClient.SetValue(key, value, TimeSpan.FromDays(30));
				}

				return typedClient.GetValue(key);
			}

			return await GetAdvocatePerformanceBreakDownForPeriod(advocateId, from, to, brandIds, period, advocateBrandIds);
		}

		private async Task<List<AdvocatePerformanceBreakDownModel>> GetAdvocatePerformanceBreakDownForPeriod(Guid advocateId, DateTime from, DateTime to, Guid[] brandIds, Period period, Guid[] advocateBrandIds)
		{
			var breakDowns = await _ticketRepository.GetAdvocatePerformanceBreakDownForPeriod(advocateId, from, to, brandIds, period);

			var allDates = period == Period.Year ? Enumerable.Range(default, MonthsInYear)
									 .Select(i => from.AddMonths(i)).ToList() : Enumerable.Range(default, (to - from).Days)
									 .Select(i => from.AddDays(i)).ToList();

			var defaultBrandBreakDown = advocateBrandIds.ToDictionary(x => x, x => default(decimal));

			var formattedResult = breakDowns.GroupBy(x => x.key)
			.Select(grp => new AdvocatePerformanceBreakDownModel()
			{
				Key = grp.Key,
				Series = grp.ToDictionary(g => g.brandId, g => g.totalPrice)
			}).ToList();

			formattedResult = (from ad in allDates
							   join fr in formattedResult
								   on ad equals fr.Key into results
							   from r in results.DefaultIfEmpty()
							   select new AdvocatePerformanceBreakDownModel()
							   {
								   Key = ad,
								   Series = r == null ? defaultBrandBreakDown : r.Series.Concat(defaultBrandBreakDown)
								   .GroupBy(x => x.Key)
								   .OrderBy(x => x.Key)
								   .ToDictionary(g => g.Key, g => g.First().Value)
							   }).ToList();

			return formattedResult;
		}

		private async Task<AdvocatePerformanceSummaryModel> GetAdvocatePerformanceSummary(Guid advocateId,
			DateTime from, DateTime to, Guid[] brandIds, Period period, Guid[] advocateBrandIds)
		{
			using var client = _redisClientsManager.GetClient();
			var brands = brandIds != null && brandIds.Any() ? String.Join("|", brandIds) : "All";
			var key = _timestampService.GetUtcTimestamp().StartOfWeek() >= to
				? AdvocatePerformanceSummaryKey(advocateId, from, period.ToString(), brands)
				: null;

			if (!string.IsNullOrEmpty(key))
			{
				if (client.ContainsKey(key))
				{
					return client.Get<AdvocatePerformanceSummaryModel>(key);
				}

				var value = await GetAdvocatePerformanceSummaryForPeriod(advocateId, from, to, brandIds, advocateBrandIds);
				return client.Set(key, value, expireIn: TimeSpan.FromDays(30));
			}

			return await GetAdvocatePerformanceSummaryForPeriod(advocateId, from, to, brandIds, advocateBrandIds);
		}

		private async Task<AdvocatePerformanceSummaryModel> GetAdvocatePerformanceSummaryForPeriod(Guid advocateId, DateTime from, DateTime to, Guid[] brandIds, Guid[] advocateBrandIds)
		{
			var summaries = await _ticketRepository.GetAdvocatePerformanceSummaryForPeriod(advocateId, from, to, brandIds);

			return new AdvocatePerformanceSummaryModel()
			{
				Brands = (from ab in advocateBrandIds
						  join s in summaries
							  on ab equals s.brandId into results
						  from r in results.DefaultIfEmpty()
						  select new BrandClosedTicketSummaryModel()
						  {
							  BrandId = ab,
							  Amount = r.totalPrice,
							  ClosedTickets = r.totalCount
						  }).ToList(),
				Total = new ClosedTicketTotalSummaryModel()
				{
					ClosedTickets = summaries.Sum(x => x.totalCount),
					Amount = summaries.Sum(x => x.totalPrice)
				}
			};
		}

	}
}
