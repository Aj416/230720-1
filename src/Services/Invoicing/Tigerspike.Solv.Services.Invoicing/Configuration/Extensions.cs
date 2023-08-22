using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Core.Configuration;

namespace Tigerspike.Solv.Services.Invoicing.Configuration
{
	public static class Extensions
	{
		public static IServiceCollection AddConfiguration(this IServiceCollection services)
		{
			using (var serviceProvider = services.BuildServiceProvider())
			{
				var configuration = serviceProvider.GetService<IConfiguration>();
				services.Configure<InvoicingOptions>(configuration.GetSection(InvoicingOptions.SectionName));
			}

			return services;
		}
	}
}
