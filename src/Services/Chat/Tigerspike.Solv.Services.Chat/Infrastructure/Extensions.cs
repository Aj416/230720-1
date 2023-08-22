using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Chat.Infrastructure.Repositories;
using Tigerspike.Solv.Services.Chat.Infrastructure.Repositories;

namespace Tigerspike.Solv.Services.Chat.Infrastructure
{
	public static class Extensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services)
		{
			services.AddScoped<IChatRepository, ChatRepository>();
			services.AddScoped<ICachedConversationRepository, CachedConversationRepository>();
			services.AddScoped<ICachedMessageWhitelistRepository, CachedMessageWhitelistRepository>();
			services.AddScoped<IMessageWhitelistRepository, MessageWhitelistRepository>();
			services.AddScoped<IChatActionRepository, ChatActionRepository>();

			return services;
		}
	}
}