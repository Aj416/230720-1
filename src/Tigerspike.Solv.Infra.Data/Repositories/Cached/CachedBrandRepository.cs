using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;
using Tigerspike.Solv.Core.Redis;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using static Tigerspike.Solv.Core.Constants.CacheKeys;

namespace Tigerspike.Solv.Infra.Data.Repositories.Cached
{
	public class CachedBrandRepository : ICachedBrandRepository
	{
		private readonly IAdvocateRepository _advocateRepository;
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly IBrandRepository _brandRepository;
		private readonly ITicketRepository _ticketRepository;
		private static readonly TimeSpan _defaultTtl = TimeSpan.FromHours(1);

		private readonly ILogger<CachedBrandRepository> _logger;
		private IMapper _mapper;
		private readonly ITimestampService _timestampService;

		public CachedBrandRepository(
			IAdvocateRepository advocateRepository,
			IRedisClientsManager redisClientsManager,
			IBrandRepository brandRepository,
			ITicketRepository ticketRepository,
			IMapper mapper,
			ITimestampService timestampService,
			ILogger<CachedBrandRepository> logger
			)
		{
			_advocateRepository = advocateRepository;
			_redisClientsManager = redisClientsManager;
			_brandRepository = brandRepository;
			_ticketRepository = ticketRepository;
			_mapper = mapper;
			_timestampService = timestampService;
			_logger = logger;
		}

		public async Task<int> GetAvailableTickets(Guid advocateId, TicketLevel level)
		{
			using var client = _redisClientsManager.GetClient();
			var total = 0;

			var advKey = AdvocateTouchedTicketsKey(advocateId);
			if (client.IsTimeToLiveSet(advKey) == false)
			{
				await PopulateTouchedTickets(client, advKey, advocateId);
			}

			var brandIds = await GetActiveBrandsIds(advocateId);
			foreach (var brandId in brandIds)
			{
				var brandKey = AvailableTicketsKey(brandId, level);
				if (client.IsTimeToLiveSet(brandKey) == false)
				{
					await PopulateBrandAvailableTickets(client, brandKey, brandId, level);
				}

				// Get the count of tickets after subtracting the rejected tickets ids for this advocate.
				total += client.GetDifferencesFromSet(brandKey, advKey).Count();
			}

			return total;
		}

		public void AddTicket(Guid ticketId, Guid brandId, TicketLevel level)
		{
			using var client = _redisClientsManager.GetClient();
			client.AddItemToSet(AvailableTicketsKey(brandId, level), ticketId.ToString());
		}

		public void TouchTicket(Guid ticketId, Guid advocateId)
		{
			using var client = _redisClientsManager.GetClient();
			client.AddItemToSet(AdvocateTouchedTicketsKey(advocateId), ticketId.ToString());
		}

		public void UntouchTicket(Guid ticketId, Guid advocateId)
		{
			using var client = _redisClientsManager.GetClient();
			client.RemoveItemFromSet(AdvocateTouchedTicketsKey(advocateId), ticketId.ToString());
		}

		public void RemoveTicket(Guid ticketId, Guid brandId)
		{
			using var client = _redisClientsManager.GetClient();
			client.RemoveItemFromSet(AvailableTicketsKey(brandId, TicketLevel.Regular), ticketId.ToString());
			client.RemoveItemFromSet(AvailableTicketsKey(brandId, TicketLevel.SuperSolver), ticketId.ToString());
		}

		/// <inheritdoc/>
		public async Task<Models.Cached.Brand> GetAsync(Guid brandId)
		{
			using var client = _redisClientsManager.GetClient();
			var key = BrandInfoKey(brandId);

			if (client.ContainsKey(key))
			{
				return client.Get<Models.Cached.Brand>(key);
			}

			var value = await _brandRepository.GetFirstOrDefaultAsync<Models.Cached.Brand>(_mapper, b => b.Id == brandId);

			return client.Set(key, value, expireIn: _defaultTtl);
		}

		/// <inheritdoc/>
		public async Task<IList<Guid>> GetActiveBrandsIds(Guid advocateId)
		{
			var setKey = AdvocateBrandsKey(advocateId);
			using var client = _redisClientsManager.GetClient();
			if (!client.ContainsKey(setKey))
			{
				var list = await _brandRepository.GetActiveBrandsIds(advocateId);
				client.AddRangeToSet(setKey, list.Select(s => s.ToString()).ToList());
				client.ExpireEntryIn(setKey, TimeSpan.FromDays(1));
			}

			return client.Sets[setKey].GetAll().Select(x => Guid.Parse(x)).ToList();
		}

		private async Task PopulateTouchedTickets(IRedisClient client, string key, Guid advocateId)
		{
			// Fetch the list of touched tickets for this advocate and save it in cache.
			var tickets = await _ticketRepository.GetTouchedTicketIds(advocateId);
			client.AddItemToSet(key, Guid.Empty.ToString()); // keep dummy item in the set, so we do not keep recalculate this if advocate hasn't touched any ticket - can be removed once https://github.com/redis/redis/issues/6048 is resolved
			client.AddRangeToSet(key, tickets.Select(x => x.ToString()).ToList());
			client.ExpireEntryAt(key, _timestampService.GetUtcTimestamp().AddMinutes(5));
		}

		private async Task PopulateBrandAvailableTickets(IRedisClient client, string key, Guid brandId, TicketLevel level)
		{
			// Fetch the list of tickets for this brand and save it in cache.
			var tickets = await _ticketRepository.GetAvailableTicketIds(brandId, level);
			client.AddItemToSet(key, Guid.Empty.ToString()); // keep dummy item in the set, so we do not keep recalculate this if there are no available tickets for the brand - can be removed once https://github.com/redis/redis/issues/6048 is resolved
			client.AddRangeToSet(key, tickets.Select(x => x.ToString()).ToList());
			client.ExpireEntryAt(key, _timestampService.GetUtcTimestamp().AddMinutes(5));
		}

	}
}
