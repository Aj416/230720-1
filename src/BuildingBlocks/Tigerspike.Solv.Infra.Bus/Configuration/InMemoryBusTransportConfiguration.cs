using System;
using System.Collections.Generic;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Tigerspike.Solv.Infra.Bus.Configuration
{

	public class InMemoryBusTransportConfiguration : BaseBusTransportConfiguration
	{
		public InMemoryBusTransportConfiguration(IOptions<BusOptions> options,
			IWebHostEnvironment environment, IServiceProvider provider,
			ILogger<AmazonBusTransportConfiguration> logger) : base(
			options, environment, provider, logger)
		{
		}

		protected override IBusControl CreateFactory(List<BusEndpointConfiguration> endpoints)
		{
			_logger.LogInformation("Configuring message bus with InMemory transport");
			return MassTransit.Bus.Factory.CreateUsingInMemory(cfg => ConfigureFactory(cfg, endpoints));
		}
	}
}
