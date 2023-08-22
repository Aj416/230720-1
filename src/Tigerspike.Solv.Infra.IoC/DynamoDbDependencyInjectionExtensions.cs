using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Infra.Bus.Model;
using Tigerspike.Solv.Infra.Data.Configuration;

namespace Tigerspike.Solv.Infra.IoC
{
	public static class DependencyInjectionExtensions
	{
		public static IServiceCollection AddDynamoDb(this IServiceCollection services,
			IConfigurationSection dynamoDbConfig)
		{
			services.Configure<DynamoDbOptions>(dynamoDbConfig);
			services.AddSingleton<IAmazonDynamoDB>(x => CreateAmazonDynamoDb(dynamoDbConfig["ServiceUrl"]));
			services.AddSingleton<IPocoDynamo, PocoDynamo>();

			services.RegisterTables();

			return services;
		}

		private static IAmazonDynamoDB CreateAmazonDynamoDb(string serviceUrl)
		{
			var clientConfig = new AmazonDynamoDBConfig
			{
				RegionEndpoint = RegionEndpoint.APSoutheast2,
				RetryMode = RequestRetryMode.Standard,
				MaxErrorRetry = 2
			};

			if (!string.IsNullOrEmpty(serviceUrl))
			{
				clientConfig.ServiceURL = serviceUrl;
			}

			var dynamoClient = new AmazonDynamoDBClient(clientConfig);

			return dynamoClient;
		}

		private static void RegisterTables(this IServiceCollection services)
		{
			IPocoDynamo db;
			DynamoDbOptions options;
			using (var serviceProvider = services.BuildServiceProvider())
			{
				var configuration = serviceProvider.GetService<IConfiguration>();
				options = configuration.GetOptions<DynamoDbOptions>(DynamoDbOptions.SectionName);
				db = serviceProvider.GetService<IPocoDynamo>();
			}

			var scheduledJobType = typeof(ScheduledJob);
			scheduledJobType.AddAttributes(new AliasAttribute(options.Tables.ScheduledJob));

			db.RegisterTable(scheduledJobType);
		}
	}
}