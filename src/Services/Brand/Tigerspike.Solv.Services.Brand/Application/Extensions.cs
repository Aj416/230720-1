using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Core.Services;

namespace Tigerspike.Solv.Services.Brand.Application
{
	public static class Extensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			// Helper services
			services.AddScoped<ITimestampService, TimestampService>();

			return services;
		}
	}
}