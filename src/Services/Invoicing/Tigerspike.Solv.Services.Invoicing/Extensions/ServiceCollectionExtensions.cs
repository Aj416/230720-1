using System;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Services.Invoicing.Configuration;

namespace Tigerspike.Solv.Services.Invoicing.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDynamoDb(this IServiceCollection services,
			IConfigurationSection configurationSection)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			services.Configure<DynamoDbOptions>(configurationSection);

			services.AddSingleton<IAmazonDynamoDB>(x => DynamoDbConfig.CreateAmazonDynamoDb(configurationSection["ServiceUrl"]));
			services.AddSingleton<IPocoDynamo, PocoDynamo>();

			return services;
		}
	}
}
