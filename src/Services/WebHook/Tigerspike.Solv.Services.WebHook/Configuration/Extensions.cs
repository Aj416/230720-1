using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tigerspike.Solv.Services.WebHook.Configuration
{
	public static class Extensions
	{
		public static IServiceCollection AddConfiguration(this IServiceCollection services)
		{
			using (var serviceProvider = services.BuildServiceProvider())
			{
				var configuration = serviceProvider.GetService<IConfiguration>();
				services.Configure<WebHookOptions>(configuration.GetSection(WebHookOptions.SectionName));
				services.Configure<DynamoDbOptions>(configuration.GetSection(DynamoDbOptions.SectionName));
				services.Configure<DynamoDbTableOptions>(configuration.GetSection(DynamoDbTableOptions.SectionName));
			}

			return services;
		}
	}
}