using System;
using System.Collections.Generic;
using Amazon.S3;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.DynamoDb;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Redis;
using Tigerspike.Solv.Core.ServiceStack;
using Tigerspike.Solv.Core.Swagger;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.Extensions;
using Tigerspike.Solv.Messaging.Chat;
using Tigerspike.Solv.Services.Chat.Application;
using Tigerspike.Solv.Services.Chat.Application.Consumers;
using Tigerspike.Solv.Services.Chat.Configuration;
using Tigerspike.Solv.Services.Chat.Extensions;
using Tigerspike.Solv.Services.Chat.Infrastructure;

namespace Tigerspike.Solv.Services.Chat
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

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
			services.AddMediator(typeof(Startup));

			// AWS
			services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
			services.AddAWSService<IAmazonS3>();
			services.AddDynamoDb(Configuration.GetSection(DynamoDbOptions.SectionName));

			services.AddBus(configurator =>
			{
				configurator.AddConsumersFromNamespaceContaining<SendAutoChatResponseCommandConsumer>();
				configurator.AddBus(provider => provider.ConfigureTransport(GetEndpoints(provider)));
			});

			services.AddRedisCache(Configuration.GetSection(RedisOptions.SectionName));

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
			app.ConfigureChatDynamoDb(createMissingTables: env.IsLocal() || env.IsDocker());
			app.ApplyDynamoDbMigrations();
		}

		private static List<BusEndpointConfiguration> GetEndpoints(IServiceProvider provider)
		{
			var options = provider.GetRequiredService<IOptions<BusOptions>>().Value;
			return new List<BusEndpointConfiguration>
			{
				new BusEndpointConfiguration
				{
					QueueName = options.Queues.Chat,
					EndpointConfigurator = c =>
					{
						c.Consumer<SendAutoChatResponseCommandConsumer>(provider);
						c.Consumer<TicketAdvocateChangedEventConsumer>(provider);
						EndpointConvention.Map<ISendAutoChatResponseCommand>(c.InputAddress);
					}
				},
			};
		}
	}
}
