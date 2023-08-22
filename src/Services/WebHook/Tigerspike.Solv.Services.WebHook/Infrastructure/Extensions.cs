using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Services.WebHook.Infrastructure.Interfaces;
using Tigerspike.Solv.Services.WebHook.Infrastructure.Repositories;

namespace Tigerspike.Solv.Services.WebHook.Infrastructure
{
	public static class Extensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services)
		{
			services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

			return services;
		}
	}
}