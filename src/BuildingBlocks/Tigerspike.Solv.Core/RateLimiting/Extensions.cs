using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Core.Redis;

namespace Tigerspike.Solv.Core.RateLimiting
{

	public static class Extensions
	{
		public static IServiceCollection AddRateLimiting(this IServiceCollection services)
		{
			using(var serviceProvider = services.BuildServiceProvider())
			{
				var configuration = serviceProvider.GetService<IConfiguration>();
				//load general configuration from appsettings.json
				services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

				//load ip rules from appsettings.json
				services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

				//load general configuration from appsettings.json
				services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));

				//load client rules from appsettings.json
				services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));

				//configure redis
				services.Configure<RedisOptions>(configuration.GetSection("Redis"));
			}

			// inject counter and rules stores
			services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
			services.AddSingleton<IRateLimitCounterStore, RedisRateLimitCounterStore>();

			services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
			services.AddSingleton<IRateLimitCounterStore, RedisRateLimitCounterStore>();

			// https://github.com/aspnet/Hosting/issues/793
			// the IHttpContextAccessor service is not registered by default.
			// the clientId/clientIp resolvers use it.
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

			return services;
		}

		public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
		{
			//use the client rate limiter when a user is authenticated or the IP one when a user is not

			builder.UseWhen(x => !x.User.Identity.IsAuthenticated, config => config.UseIpRateLimiting());
			builder.UseWhen(x => x.User.Identity.IsAuthenticated, config => config.UseClientRateLimiting());

			return builder;
		}

		public static IWebHost SeedRateLimitingRules(this IWebHost webHost)
		{

			using(var scope = webHost.Services.CreateScope())
			{
				// get the IpPolicyStore instance
				var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();

				// seed IP data from appsettings
				ipPolicyStore.SeedAsync().Wait();

				// get the ClientPolicyStore instance
				var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();

				// seed client data from appsettings
				clientPolicyStore.SeedAsync().Wait();
			}
			return webHost;
		}
	}

}