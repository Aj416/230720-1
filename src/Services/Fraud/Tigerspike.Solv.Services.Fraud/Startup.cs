using System;
using System.Collections.Generic;
using Amazon.S3;
using AutoMapper;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Swagger;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.Extensions;
using Tigerspike.Solv.Services.Fraud.Extensions;
using Tigerspike.Solv.Services.Fraud.Application.Consumers;
using MediatR;
using Tigerspike.Solv.Core.ServiceStack;
using Tigerspike.Solv.Services.Fraud.Application;
using Tigerspike.Solv.Services.Fraud.Configuration;
using Tigerspike.Solv.Services.Fraud.Infrastructure;

namespace Tigerspike.Solv.Services.Fraud
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public virtual void ConfigureServices(IServiceCollection services)
		{
			services.AddWebApi(fluentValidation => {});
			services.AddHealthChecks();
			services.AddOptions();
			services.AddSwaggerDocs();

			// AutoMapper
			services.AddAutoMapper(
				typeof(Application.AutoMapper.AutoMapperConfig)
			);

			// Adding MediatR for Domain Events and Notifications
			services.AddMediatR(typeof(Startup));

			// AWS
			services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
			services.AddAWSService<IAmazonS3>();
			services.AddDynamoDb(Configuration.GetSection(DynamoDbOptions.SectionName));


			services.AddBus(configurator =>
			{
				configurator.AddConsumersFromNamespaceContaining<DetectFraudConsumer>();
				configurator.AddBus(provider => provider.ConfigureTransport(GetEndpoints(provider)));
			});

			services.AddConfiguration();
			services.AddInfrastructure();
			services.AddApplication();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseWebApiEndpoints(endpoints =>
			{
				endpoints.UseDefaults();
			}, false);
			app.UseHealthChecks();
			app.UseStaticFiles();
			app.UseSwaggerDocs();
			app.UseServiceStack();
			app.ConfigureFraudDynamoDb(createMissingTables: env.IsLocal() || env.IsDocker());
			app.ApplyDynamoDbMigrations();
		}

		private List<BusEndpointConfiguration> GetEndpoints(IServiceProvider provider)
		{
			var options = provider.GetRequiredService<IOptions<BusOptions>>().Value;
			return new List<BusEndpointConfiguration>
			{
				new BusEndpointConfiguration
				{
					QueueName = options.Queues.Fraud,
					EndpointConfigurator = c =>
					{
						c.ConfigureConsumeTopology = false;
						c.UseConcurrencyLimit(5);
						c.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(5)));
						c.Consumer<DetectFraudConsumer>(provider);
					}
				}
			};
		}
	}
}