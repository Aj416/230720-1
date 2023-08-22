using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Tigerspike.Solv.Core.Redis
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddRedisCache(this IServiceCollection services,
			IConfigurationSection redisConfig)
		{
			var readWriteHosts = redisConfig[nameof(RedisOptions.ReadWriteHosts)]
				.Split(',')
				.Where(n => !string.IsNullOrWhiteSpace(n))
				.Select(n => n.Trim())
				.ToList();

			services.AddDistributedRedisCache(options =>
			{
				options.ConfigurationOptions = new ConfigurationOptions();

				foreach (var readWriteHost in readWriteHosts)
				{
					options.ConfigurationOptions.EndPoints.Add(readWriteHost);
				}

				options.ConfigurationOptions.Ssl = redisConfig
					.GetValue<bool>(nameof(RedisOptions.Ssl));
			});

			services.AddServiceStackRedis(redisConfig);

			return services;
		}
	}
}