using System;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Services.Fraud.Configuration;
using Tigerspike.Solv.Services.Fraud.Models;

namespace Tigerspike.Solv.Services.Fraud.Extensions
{
	public static class FraudServiceCollectionExtensions
	{
		public static FraudServicesBuilder AddDynamoDb(this IServiceCollection services,
			IConfigurationSection configurationSection)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));

			var builder = new FraudServicesBuilder(services);

			services.Configure<DynamoDbOptions>(configurationSection);

			services.AddSingleton<IAmazonDynamoDB>(x => DynamoDbConfig.CreateAmazonDynamoDb(configurationSection["ServiceUrl"]));
			services.AddSingleton<IPocoDynamo, PocoDynamo>();

			return builder;
		}

		public class FraudServicesBuilder
		{
			public IServiceCollection Services { get; private set; }

			internal FraudServicesBuilder(IServiceCollection services)
			{
				Services = services;
			}
		}
	}
}