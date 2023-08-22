using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using Tigerspike.Solv.Core.Redis;

namespace Tigerspike.Solv.Core.RateLimiting
{
	public class RedisRateLimitCounterStore : IRateLimitCounterStore
	{
		private readonly RedisOptions _redisOptions;
		private readonly ConnectionMultiplexer _redis;
		private readonly ILogger _logger;

		public RedisRateLimitCounterStore(
			IOptions<RedisOptions> redisOptions,
			ILogger<RedisRateLimitCounterStore> logger
		)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("Logger should be specified");
			}

			_logger = logger;
			_redisOptions = redisOptions?.Value;

			var readWriteHosts = _redisOptions?.ReadWriteHosts
				.Split(',')
				.Where(n => !string.IsNullOrWhiteSpace(n))
				.Select(n => n.Trim())
				.ToList();

			if (readWriteHosts == null)
			{
				throw new ArgumentNullException(nameof(redisOptions));
			}

			var configurationOptions = new ConfigurationOptions();

			foreach (var readWriteHost in readWriteHosts)
			{
				configurationOptions.EndPoints.Add(readWriteHost);
			}

			_redis = ConnectionMultiplexer.Connect(configurationOptions);
		}

		private IDatabase RedisDatabase => _redis.GetDatabase();

		public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await TryRedisCommandAsync(
				() =>
				{
					return RedisDatabase.KeyExistsAsync(id);
				});
		}

		public async Task<RateLimitCounter?> GetAsync(string id, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await TryRedisCommandAsync(
				async() =>
				{
					var value = await RedisDatabase.StringGetAsync(id);

					if (!string.IsNullOrEmpty(value))
					{
						return JsonConvert.DeserializeObject<RateLimitCounter?>(value);
					}

					return null;
				});
		}

		public async Task RemoveAsync(string id, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			_ = await TryRedisCommandAsync(
				async() =>
				{
					await RedisDatabase.KeyDeleteAsync(id);

					return true;
				});
		}

		public async Task SetAsync(string id, RateLimitCounter? entry, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			_ = await TryRedisCommandAsync(
				async() =>
				{
					await RedisDatabase.StringSetAsync(id, JsonConvert.SerializeObject(entry.Value), expirationTime);

					return true;
				});
		}

		private async Task<T> TryRedisCommandAsync<T>(Func<Task<T>> command)
		{

			try
			{
				return await command();
			}
			catch (Exception ex)
			{
				_logger.LogError($"Redis Rate Limiter command failed: {ex}");
				return default(T);
			}

		}
	}
}