using System;
using System.Threading.Tasks;
using ServiceStack.Redis;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;

namespace Tigerspike.Solv.Infra.Data.Repositories.Cached
{
	public class CachedUserRepository : ICachedUserRepository
	{
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly IUserRepository _userRepository;

		public CachedUserRepository(
			IRedisClientsManager redisClientsManager,
			IUserRepository userRepository
			)
		{
			_redisClientsManager = redisClientsManager;
			_userRepository = userRepository;
		}

		public async Task<bool?> IsUserEnabled(Guid userId)
		{
			using (var redisClient = _redisClientsManager.GetClient())
			{
				var cacheKey = CacheKeys.UserEnabledKey(userId);
				if (redisClient.ContainsKey(cacheKey) == false)
				{
					var cacheValue = await _userRepository.IsUserEnabled(userId);
					redisClient.Set(cacheKey, cacheValue, TimeSpan.FromDays(1));
				}

				return redisClient.Get<bool?>(cacheKey);
			}
		}

	}
}
