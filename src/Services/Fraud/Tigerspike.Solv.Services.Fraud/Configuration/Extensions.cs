using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tigerspike.Solv.Services.Fraud.Configuration
{
	public static class Extensions
	{
		public static IServiceCollection AddConfiguration(this IServiceCollection services)
		{
			using (var serviceProvider = services.BuildServiceProvider())
			{
				var configuration = serviceProvider.GetService<IConfiguration>();
				services.Configure<DynamoDbOptions>(configuration.GetSection(DynamoDbOptions.SectionName));
				services.Configure<DynamoDbTableOptions>(configuration.GetSection(DynamoDbTableOptions.SectionName));
				services.Configure<ElasticSearchOptions>(configuration.GetSection(ElasticSearchOptions.SectionName));
			}

			return services;
		}
	}
}