using System;
using System.Collections.Generic;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Swagger;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.Extensions;
using Tigerspike.Solv.Services.Notification.Application;
using Tigerspike.Solv.Services.Notification.Application.Commands;
using Tigerspike.Solv.Services.Notification.Configuration;

namespace Tigerspike.Solv.Services.Notification
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
			services.AddOptions();
			services.AddSwaggerDocs();
			services.AddEmailProvider();
			services.AddBus(configurator =>
			{
				configurator.AddConsumersFromNamespaceContaining<SendEmailMessageConsumer>();
				configurator.AddConsumersFromNamespaceContaining<SendMessengerMessageConsumer>();
				configurator.AddBus(provider => provider.ConfigureTransport(GetEndpoints(provider)));
			});
			services.AddConfiguration();
			services.AddApplication();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseWebApiEndpoints(endpoints =>
			{
				endpoints.UseDefaults();
			}, true);
			app.UseStaticFiles();
			app.UseSwaggerDocs();
		}

		private List<BusEndpointConfiguration> GetEndpoints(IServiceProvider provider)
		{
			var options = provider.GetRequiredService<IOptions<BusOptions>>().Value;
			return new List<BusEndpointConfiguration>
			{
				new BusEndpointConfiguration
				{
					QueueName = options.Queues.Notification,
					EndpointConfigurator = c =>
					{
						c.UseConcurrencyLimit(5);
						c.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(5)));
						c.Consumer<SendEmailMessageConsumer>(provider);
						c.Consumer<SendMessengerMessageConsumer>(provider);
					}
				}
			};
		}
	}
}