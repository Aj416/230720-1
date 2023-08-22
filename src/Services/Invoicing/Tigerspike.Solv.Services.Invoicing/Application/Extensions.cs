using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Services.Invoicing.Application.CommandHandlers;
using Tigerspike.Solv.Services.Invoicing.Application.EventHandlers;
using Tigerspike.Solv.Services.Invoicing.Application.Services;

namespace Tigerspike.Solv.Services.Invoicing.Application
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

			services.AddScoped<IInvoiceService, InvoiceService>();
			// Helper services
			services.AddScoped<ITimestampService, TimestampService>();

			// all notification handlers
			services.Scan(scan => scan
				.FromAssembliesOf(typeof(InvoiceEventHandler))
				.AddClasses(x => x.AssignableTo(typeof(INotificationHandler<>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime()
			);

			// all request handlers
			services.Scan(scan => scan
				.FromAssembliesOf(typeof(InvoiceCommandHandler))
				.AddClasses(x => x.AssignableTo(typeof(IRequestHandler<,>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime()
			);

			services.AddScoped<IMapper>(
				sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
			return services;
		}
	}
}
