using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Redis;

namespace Tigerspike.Solv.Core.Redis
{
	public static class RedisDependencyInjectionExtensions
	{
		public static IServiceCollection AddServiceStackRedis(this IServiceCollection services,
			IConfigurationSection redisConfig)
		{
			var readWriteHosts = redisConfig[nameof(RedisOptions.ReadWriteHosts)]
				.Split(',')
				.Where(n => !string.IsNullOrWhiteSpace(n))
				.Select(n => n.Trim())
				.ToList();

			var readOnlyHosts = redisConfig[nameof(RedisOptions.ReadOnlyHosts)]
				.Split(',')
				.Where(n => !string.IsNullOrWhiteSpace(n))
				.Select(n => n.Trim())
				.AsEnumerable();

			services.AddSingleton<IRedisClientsManager>(c => new PooledRedisClientManager(readWriteHosts, readOnlyHosts));

			return services;
		}
	}
}
