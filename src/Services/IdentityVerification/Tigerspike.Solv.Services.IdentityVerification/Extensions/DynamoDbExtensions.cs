using System;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Core.DynamoDb;
using Tigerspike.Solv.Services.IdentityVerification.Configuration;

namespace Tigerspike.Solv.Services.IdentityVerification.Extensions
{
	public static class DynamoDbExtensions
	{
		public static IServiceCollection AddDynamoDb(this IServiceCollection services,
			IConfigurationSection configurationSection)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			services.Configure<DynamoDbOptions>(configurationSection);

			services.AddSingleton<IAmazonDynamoDB>(x =>
				Core.DynamoDb.DynamoDbExtensions.CreateAmazonDynamoDb(configurationSection["ServiceUrl"]));
			services.AddSingleton<IPocoDynamo, PocoDynamo>();

			return services;
		}

		public static void ConfigureChatDynamoDb(this IApplicationBuilder builder, bool createMissingTables)
		{
			var serviceProvider = builder.ApplicationServices;
			var db = serviceProvider.GetService<IPocoDynamo>();
			var settingsAccessor = serviceProvider.GetService<IOptions<DynamoDbOptions>>();

			DynamoDbConfig.InitSchema(db, settingsAccessor.Value, createMissingTables);
		}

		public static void ApplyDynamoDbMigrations(this IApplicationBuilder builder)
		{
			// add migration scripts here
		}
	}
}