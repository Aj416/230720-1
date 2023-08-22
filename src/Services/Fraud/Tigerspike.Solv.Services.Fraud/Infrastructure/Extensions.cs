using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces;
using Tigerspike.Solv.Services.Fraud.Infrastructure.Repositories;

namespace Tigerspike.Solv.Services.Fraud.Infrastructure
{
	public static class Extensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services)
		{
			services.AddScoped<ITicketRepository, TicketRepository>();
			services.AddScoped<IRuleRepository, RuleRepository>();
			services.AddScoped<ITicketDetectionRepository, TicketDetectionRepository>();

			return services;
		}
	}
}