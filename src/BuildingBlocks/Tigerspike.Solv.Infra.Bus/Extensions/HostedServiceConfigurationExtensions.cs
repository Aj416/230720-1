using System;
using System.Collections.Generic;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.Exceptions;
using Tigerspike.Solv.Infra.Bus.HealthChecks;
using Tigerspike.Solv.Infra.Bus.Scheduler;

namespace Tigerspike.Solv.Infra.Bus.Extensions
{
	public static class HostedServiceConfigurationExtensions
	{

		/// <summary>
		/// Uses the health check, in combination with the MassTransit Hosted Service, to monitor the bus
		/// </summary>
		/// <param name="busConfigurator"></param>
		/// <param name="provider"></param>
		public static void UseHealthCheck(this IBusFactoryConfigurator busConfigurator, IServiceProvider provider)
		{
			var healthCheck = provider.GetRequiredService<BusHealthCheck>();

			busConfigurator.ConnectBusObserver(healthCheck);
		}

		/// <summary>
		/// Adds the MassTransit <see cref="IHostedService"/>, which includes a bus and endpoint health check
		/// </summary>
		/// <param name="services"></param>
		/// <param name="includeScheduler">Whether to include the scheduler.</param>
		private static void AddBusHostedService(this IServiceCollection services, bool includeScheduler)
		{
			var busCheck = new BusHealthCheck();
			var receiveEndpointCheck = new ReceiveEndpointHealthCheck();

			var healthCheckOptions = HealthCheckOptions.Default;

			services.AddSingleton(busCheck);

			services.AddHealthChecks()
				.AddBusHealthCheck(healthCheckOptions.BusHealthCheckName, busCheck)
				.AddBusHealthCheck(healthCheckOptions.ReceiveEndpointHealthCheckName, receiveEndpointCheck);


			services.AddSingleton<IHostedService>(p =>
			{
				var busControl = p.GetRequiredService<IBusControl>();
				var logger = p.GetService<ILogger<BusHostedService>>();
				var options = p.GetRequiredService<IOptions<BusOptions>>();

				return new BusHostedService(busControl, new SimplifiedBusHealthCheck(), receiveEndpointCheck, options, logger);
			});
		}

		public static void AddBus(this IServiceCollection services, Action<IServiceCollectionBusConfigurator> configurator = null)
		{
			DatabaseOptions databaseOptions;
			BusOptions busOptions;

			using (var serviceProvider = services.BuildServiceProvider())
			{
				var configuration = serviceProvider.GetService<IConfiguration>();
				services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
				services.Configure<BusOptions>(configuration.GetSection(BusOptions.SectionName));
				databaseOptions = configuration.GetOptions<DatabaseOptions>(DatabaseOptions.SectionName);
				busOptions = configuration.GetOptions<BusOptions>(BusOptions.SectionName);
			}

			services.AddTransportConfiguration();

			services.AddMassTransit(configurator);

			services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
			services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
			services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
			services.AddTransient<ISchedulerService, SchedulerService>();

			services.AddBusHostedService(busOptions.IncludeScheduler);
		}

		public static void AddMessageScheduler(this IServiceCollectionBusConfigurator configurator)
		{
			BusOptions busOptions;
			using (var serviceProvider = configurator.Collection.BuildServiceProvider())
			{
				var configuration = serviceProvider.GetService<IConfiguration>();
				busOptions = configuration.GetOptions<BusOptions>(BusOptions.SectionName);
			}

			if (busOptions.IncludeScheduler)
			{
				configurator.AddMessageScheduler(new Uri($"queue:{busOptions.Queues.Quartz}"));
			}
		}

		public static IBusControl ConfigureTransport(this IBusRegistrationContext provider, List<BusEndpointConfiguration> endpoints)
		{
			var transport = provider.GetService<IBusTransportConfiguration>();
			return transport.Configure(endpoints);
		}

		private static void AddTransportConfiguration(this IServiceCollection services)
		{
			BusOptions options;

			using (var serviceProvider = services.BuildServiceProvider())
			{
				var configuration = serviceProvider.GetService<IConfiguration>();
				services.Configure<BusOptions>(configuration.GetSection(BusOptions.SectionName));
				options = configuration.GetOptions<BusOptions>(BusOptions.SectionName);
			}
			switch (options.Transport)
			{
				case "amazonsqs":
					services.AddSingleton<IBusTransportConfiguration, AmazonBusTransportConfiguration>();
					return;
				case "in-memory":
					services.AddSingleton<IBusTransportConfiguration, InMemoryBusTransportConfiguration>();
					return;
				default:
					throw new BusConfigurationException($"Unkown bus transport: {options.Transport}");
			}
		}

		static IHealthChecksBuilder AddBusHealthCheck(this IHealthChecksBuilder builder, string suffix, IHealthCheck healthCheck)
		{
			return builder.AddCheck($"bus-{suffix}", healthCheck, HealthStatus.Unhealthy, new[] {"ready"});
		}
	}
}