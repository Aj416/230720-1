using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Core.Notifications;

namespace Tigerspike.Solv.Core.Mediator
{
	public static class Extensions
	{
		public static IServiceCollection AddMediator(
			this IServiceCollection services,
			params Type[] handlerAssemblyMarkerTypes)
		{

			services.AddMediatR(handlerAssemblyMarkerTypes, null);

			services.AddScoped<IMediatorHandler, InMemoryBus>();
			services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
			services.AddScoped<IDomainNotificationHandler, DomainNotificationHandler>();

			services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>));

			return services;
		}
	}
}