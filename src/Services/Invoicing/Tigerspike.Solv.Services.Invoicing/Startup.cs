using System;
using System.Collections.Generic;
using Amazon.S3;
using GreenPipes;
using Hellang.Middleware.ProblemDetails;
using MassTransit;
using MassTransit.AmazonSqsTransport;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Exceptions;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Swagger;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.Extensions;
using Tigerspike.Solv.Messaging.Invoicing;
using Tigerspike.Solv.Services.Invoicing.Application;
using Tigerspike.Solv.Services.Invoicing.Application.Consumers;
using Tigerspike.Solv.Services.Invoicing.Configuration;
using Tigerspike.Solv.Services.Invoicing.Context;
using Tigerspike.Solv.Services.Invoicing.Extensions;
using Tigerspike.Solv.Services.Invoicing.Infrastructure;

namespace Tigerspike.Solv.Services.Invoicing
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddWebApi(fluentValidation => { });
			services.AddOptions();
			services.AddSwaggerDocs();
			// AutoMapper
			services.AddAutoMapper(
				typeof(Application.AutoMapper.AutoMapperConfig)
			);
			// Adding MediatR for Domain Events and Notifications
			services.AddMediatR(typeof(Startup));

			services.AddHostedService<MigrateHostedService>();

			// AWS
			services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
			services.AddAWSService<IAmazonS3>();
			services.AddDynamoDb(Configuration.GetSection(DynamoDbOptions.SectionName));

			services.AddContext();
			services.AddBus(configurator =>
			{
				configurator.AddMessageScheduler();
				configurator.AddConsumersFromNamespaceContaining<NewRecurringInvoicingCycleConsumer>();
				configurator.AddBus(provider => provider.ConfigureTransport(GetEndpoints(provider)));
				configurator.AddRequestClient<IBrandInfoCommand>();
				configurator.AddRequestClient<IBrandIdForInvoicingCommand>();
				configurator.AddRequestClient<IFetchClientInvoicingAmountCommand>();
				configurator.AddRequestClient<IFetchBrandBillingDetailCommand>();
				configurator.AddRequestClient<IFetchAdvocateIdsForInvoicingCommand>();
				configurator.AddRequestClient<IFetchAdvocatesToInvoiceCommand>();
				configurator.AddRequestClient<IFetchTicketsForAdvocateInvoiceCommand>();
				configurator.AddRequestClient<IFetchBrandIdsForInvoicingCommand>();
				configurator.AddRequestClient<IFetchAdvocateInfoCommand>();
				configurator.AddRequestClient<IFetchTicketInfoCommand>();
				configurator.AddRequestClient<IFetchTicketsInfoForAdvocateInvoiceCommand>();
				configurator.AddRequestClient<IFetchAdvocateDetailsCommand>();
				configurator.AddRequestClient<IRiskTransactionContextCommand>();
				configurator.AddRequestClient<IExecutePaymentCommand>();
				configurator.AddRequestClient<IFetchPaymentReceiverAccountIdCommand>();
				configurator.AddRequestClient<IStartInvoicingCycleCommand>();
			});

			services.AddConfiguration();
			services.AddInfrastructure();
			services.AddApplication();
			services.AddExceptionHandling();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseWebApiEndpoints(endpoints =>
			{
				endpoints.UseDefaults();
			}, true);
			app.UseSwaggerDocs();

			// Use problem details exception handler.
			app.UseProblemDetails();

			app.ConfigureDynamoDb(createMissingTables: false);
			app.ApplyDynamoDbMigrations();
		}

		private static List<BusEndpointConfiguration> GetEndpoints(IServiceProvider provider)
		{
			var options = provider.GetRequiredService<IOptions<BusOptions>>().Value;
			return new List<BusEndpointConfiguration>
			 {
			 	new BusEndpointConfiguration
			 	{
			 		QueueName = options.Queues.NewInvoicing,
			 		EndpointConfigurator = c =>
			 		{
			 			c.Consumer<NewStartInvoicingCycleConsumer>(provider);
						c.Consumer<NewGenerateInvoicingCycleInvoicesConsumer>(provider);
						c.Consumer<NewCreateClientInvoiceConsumer>(provider);
						c.Consumer<NewCreateAdvocateInvoiceConsumer>(provider);
			 			EndpointConvention.Map<IStartInvoicingCycleCommand>(c.InputAddress);
						EndpointConvention.Map<IGenerateInvoicingCycleInvoicesCommand>(c.InputAddress);
						EndpointConvention.Map<ICreateClientInvoiceCommand>(c.InputAddress);
						EndpointConvention.Map<ICreateAdvocateInvoiceCommand>(c.InputAddress);

						if (c is MassTransit.AmazonSqsTransport.IQueueConfigurator sqsConfigurator)
						{
							// Amazon SQS specific configuration to allow messages to be consumed for longer period than default 30 seconds
							sqsConfigurator.QueueAttributes["VisibilityTimeout"] = TimeSpan.FromMinutes(15).TotalSeconds;
						}

						if (c is IAmazonSqsReceiveEndpointConfigurator configurator)
						{
							configurator.PrefetchCount = 1; // take one message at a time from SQS
						}

						c.UseConcurrencyLimit(1); // do not process import items in parallel
						c.UseRateLimit(100, TimeSpan.FromMinutes(1)); // limit number of items to be processed in a period of time, so the we would not overwhelm ourselves
			 		}
			 	}
			 };
		}
	}
}
