using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Infra.Bus;
using Tigerspike.Solv.Services.Fraud.Application.Services;
using Tigerspike.Solv.Services.Fraud.Models;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Tigerspike.Solv.Services.Fraud.Application
{
	public static class Extensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			// mediator
			services.AddScoped<IDomainNotificationHandler, DomainNotificationHandler>();
			services.AddScoped<INotificationHandler<DomainNotification>>(
				x => x.GetService<IDomainNotificationHandler>());
			services.AddScoped<IEventStore, InMemoryEventStore>();
			services.AddScoped<IMediatorHandler, InMemoryBus>();
			services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>));

			// services
			services.AddTransient<IFraudService, FraudService>();
			services.AddScoped<ISearchService<FraudSearchModel>, FraudSearchService>();

			services.AddScoped<IMapper>(
				sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));

			return services;
		}
	}
}