using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack.Redis;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;

namespace Tigerspike.Solv.Infra.Data.Repositories.Cached
{
	public class CachedBrandAdvocateResponseConfigRepository : ICachedBrandAdvocateResponseConfigRepository
	{
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly IBrandAdvocateResponseConfigRepository _brandAdvocateResponseConfigRepository;

		public CachedBrandAdvocateResponseConfigRepository(
			IRedisClientsManager redisClientsManager,
			IBrandAdvocateResponseConfigRepository brandAdvocateResponseConfigRepository
			)
		{
			_redisClientsManager = redisClientsManager;
			_brandAdvocateResponseConfigRepository = brandAdvocateResponseConfigRepository;
		}

		public async Task<IEnumerable<BrandAdvocateResponseConfig>> Get(Guid brandId)
		{
			using (var redisClient = _redisClientsManager.GetClient())
			{
				var typedClient = redisClient.As<IEnumerable<BrandAdvocateResponseConfig>>();
				var cacheKey = CacheKeys.GetBrandResponsesKey(brandId);
				if (typedClient.ContainsKey(cacheKey) == false)
				{
					var cacheValue = await _brandAdvocateResponseConfigRepository.Get(brandId);
					typedClient.SetValue(cacheKey, cacheValue, TimeSpan.FromDays(1));
				}

				return typedClient.GetValue(cacheKey);
			}
		}

	}
}
