using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Core.Behaviours;

namespace Tigerspike.Solv.Services.Invoicing.Context
{
	public static class Extensions
	{
		public static IServiceCollection AddContext(this IServiceCollection services)
		{
			services.AddScoped<DbContext, InvoicingDbContext>();
			services.AddDbContext<InvoicingDbContext>();
			services.AddScoped<IContextBehaviour, CreatedDateBehaviour>();
			services.AddScoped<IContextBehaviour, ModifiedDateBehaviour>();
			return services;
		}
	}
}
