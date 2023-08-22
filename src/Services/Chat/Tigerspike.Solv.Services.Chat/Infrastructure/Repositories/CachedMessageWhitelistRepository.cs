using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Redis;
using Tigerspike.Solv.Core.Redis;
using Tigerspike.Solv.Core.Services;
using static Tigerspike.Solv.Core.Constants.CacheKeys;

namespace Tigerspike.Solv.Chat.Infrastructure.Repositories
{
	public class CachedMessageWhitelistRepository : ICachedMessageWhitelistRepository
	{
		private readonly IMessageWhitelistRepository _messageWhitelistRepository;
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly ITimestampService _timestampService;

		public CachedMessageWhitelistRepository(
			IMessageWhitelistRepository messageWhitelistRepository,
			ITimestampService timestampService, IRedisClientsManager redisClientsManager)
		{
			_messageWhitelistRepository = messageWhitelistRepository;
			_timestampService = timestampService;
			_redisClientsManager = redisClientsManager;
		}

		/// <inheritdoc />
		public List<string> GetList(Guid brandId)
		{
			var ttl = _timestampService.GetUtcTimestamp() + _defaultTtl;
			var key = GetBrandWhiteListKey(brandId);

			using var client = _redisClientsManager.GetClient();

			if (client.ContainsKey(key))
			{
				return client.GetList<string>(key);
			}

			return client.SetList(key,
				_messageWhitelistRepository.GetList(brandId).Select(m => m.Phrase).ToList(),
				expireAt: ttl);
		}

		/// <inheritdoc />
		public void Invalidate(Guid brandId)
		{
			using var client = _redisClientsManager.GetClient();
			client.Remove(GetBrandWhiteListKey(brandId));
		}


		private static readonly TimeSpan _defaultTtl = TimeSpan.FromDays(30);
	}
}