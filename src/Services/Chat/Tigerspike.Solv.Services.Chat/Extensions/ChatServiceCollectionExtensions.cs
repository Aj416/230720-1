using System;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Core.DynamoDb;

namespace Tigerspike.Solv.Services.Chat.Extensions
{
    public static class ChatServiceCollectionExtensions
    {
        public static ChatServicesBuilder AddDynamoDb(this IServiceCollection services,
            IConfigurationSection configurationSection)
        {
            if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			var builder = new ChatServicesBuilder(services);

            services.Configure<DynamoDbOptions>(configurationSection);

			services.AddSingleton<IAmazonDynamoDB>(x => DynamoDbExtensions.CreateAmazonDynamoDb(configurationSection["ServiceUrl"]));
			services.AddSingleton<IPocoDynamo, PocoDynamo>();

            return builder;
        }

        public class ChatServicesBuilder
        {
            public IServiceCollection Services { get; private set; }

			internal ChatServicesBuilder(IServiceCollection services) => Services = services;
		}
    }
}