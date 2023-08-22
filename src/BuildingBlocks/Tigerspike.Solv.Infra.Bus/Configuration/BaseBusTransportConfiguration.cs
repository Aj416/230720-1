using System;
using System.Collections.Generic;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Infra.Bus.Extensions;

namespace Tigerspike.Solv.Infra.Bus.Configuration
{

	public abstract class BaseBusTransportConfiguration : IBusTransportConfiguration
	{
		private protected readonly BusOptions _options;
		private protected readonly ILogger _logger;
		private protected readonly IWebHostEnvironment _environment;
		private protected readonly IServiceProvider _provider;

		protected BaseBusTransportConfiguration(IOptions<BusOptions> options,
			IWebHostEnvironment environment, IServiceProvider provider, ILogger logger)
		{
			_options = options.Value;
			_environment = environment;
			_provider = provider;
			_logger = logger;
		}

		public IBusControl Configure(List<BusEndpointConfiguration> endpoints)
		{
			return CreateFactory(endpoints);
		}

		protected abstract IBusControl CreateFactory(List<BusEndpointConfiguration> endpoints);

		protected virtual void ConfigureFactory(IBusFactoryConfigurator busFactoryConfigurator, List<BusEndpointConfiguration> endpoints)
		{
			busFactoryConfigurator.ConfigureJsonSerializer(settings => settings.AsDefault()); // use platform-wide serializing settings
			busFactoryConfigurator.UseHealthCheck(_provider);

			ConfigureEndpoints(busFactoryConfigurator, endpoints);
		}

		private void ConfigureEndpoints(IBusFactoryConfigurator busFactoryConfigurator, List<BusEndpointConfiguration> endpoints)
		{
			endpoints?.ForEach(e =>
			{
				busFactoryConfigurator.ReceiveEndpoint(e.QueueName, e.EndpointConfigurator);
			});
		}
	}
}
