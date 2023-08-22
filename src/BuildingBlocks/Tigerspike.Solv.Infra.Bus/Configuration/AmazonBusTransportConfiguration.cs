using System;
using System.Collections.Generic;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using MassTransit.AmazonSqsTransport;
using MassTransit.AmazonSqsTransport.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Tigerspike.Solv.Infra.Bus.Configuration
{

	public class AmazonBusTransportConfiguration : BaseBusTransportConfiguration
	{
		public AmazonBusTransportConfiguration(IOptions<BusOptions> options,
			IWebHostEnvironment environment, IServiceProvider provider,
			ILogger<AmazonBusTransportConfiguration> logger) : base(
			options, environment, provider, logger)
		{
		}

		protected override IBusControl CreateFactory(List<BusEndpointConfiguration> endpoints)
		{
			_logger.LogInformation("Configuring message bus with Amazon-SQS transport");
			return MassTransit.Bus.Factory.CreateUsingAmazonSqs(cfg => ConfigureFactory(cfg, endpoints));
		}

		protected override void ConfigureFactory(IBusFactoryConfigurator busFactoryConfigurator,
			List<BusEndpointConfiguration> endpoints)
		{
			var amazonBusFactoryConfigurator = (IAmazonSqsBusFactoryConfigurator) (busFactoryConfigurator);
			amazonBusFactoryConfigurator.PrefetchCount = 10;
			amazonBusFactoryConfigurator.WaitTimeSeconds = 1;
			amazonBusFactoryConfigurator.OverrideDefaultBusEndpointQueueName($"{_environment.EnvironmentName}-default");

			_logger.LogDebug($"Checking bus environment with Service url {_options.Sqs?.ServiceUrl} for env");

			if (_options.UseServiceUrl)
			{
				// configure for localstack
				amazonBusFactoryConfigurator.Host(
					new Uri(_options.Protocol + "://" + new Uri(_options.Sqs.ServiceUrl).Host), options =>
					{
						options.Config(new AmazonSQSConfig {ServiceURL = _options.Sqs.ServiceUrl});
						options.Config(new AmazonSimpleNotificationServiceConfig
							{ServiceURL = _options.Sns.ServiceUrl});
						options.AccessKey(_options.AccessKey);
						options.SecretKey(_options.SecretKey);

						options.Scope(_environment.EnvironmentName, true);
					});
			}
			else
			{
				_logger.LogDebug("Setting access key and secret key to bus");
				// configure for real AWS
				amazonBusFactoryConfigurator.Host(_options.Region, options =>
				{
					options.AccessKey(_options.AccessKey);
					options.SecretKey(_options.SecretKey);

					options.Scope(_environment.EnvironmentName, true);
				});
			}

			base.ConfigureFactory(busFactoryConfigurator, endpoints);
		}
	}
}
