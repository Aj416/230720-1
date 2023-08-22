using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Chat.Application.ChatCensor;
using Tigerspike.Solv.Chat.Application.ChatCensor.Strategies;
using Tigerspike.Solv.Chat.Application.CommandHandlers;
using Tigerspike.Solv.Chat.Application.Services;
using Tigerspike.Solv.Chat.Infrastructure;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Services.Chat.Application.Services;

namespace Tigerspike.Solv.Services.Chat.Application
{
	public static class Extensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddScoped<IAuthorizationService, AuthorizationService>();
			services.AddScoped<IChatService, ChatService>();

			// chat censor
			services.AddTransient<IChatCensorContext, ChatCensorContext>();
			services.Scan(scan => scan
				.FromAssembliesOf(typeof(ChatCensorContext))
				.AddClasses(x => x.AssignableTo(typeof(IChatCensorStrategy)))
				.AsImplementedInterfaces()
				.WithTransientLifetime()
			);

			// all request handlers
			services.Scan(scan => scan
				.FromAssembliesOf(typeof(ChatCommandHandler))
				.AddClasses(x => x.AssignableTo(typeof(IRequestHandler<,>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime()
			);

			// Helper services
			services.AddScoped<ITimestampService, TimestampService>();

			return services;
		}
	}
}