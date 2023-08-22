using Amazon;
using Amazon.DynamoDBv2;
using ServiceStack;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Infra.Bus.Model;

namespace Tigerspike.Solv.Services.Invoicing.Configuration
{
	public static class DynamoDbConfig
	{
		public static IAmazonDynamoDB CreateAmazonDynamoDb(string serviceUrl)
		{
			var clientConfig = new AmazonDynamoDBConfig { RegionEndpoint = RegionEndpoint.APSoutheast2 };

			if (!string.IsNullOrEmpty(serviceUrl))
			{
				clientConfig.ServiceURL = serviceUrl;
			}

			var dynamoClient = new AmazonDynamoDBClient(clientConfig);

			return dynamoClient;
		}

		public static void InitSchema(IPocoDynamo db, DynamoDbOptions options, bool createMissingTables)
		{
			var scheduledJobEntityType = typeof(ScheduledJob);
			scheduledJobEntityType.AddAttributes(new AliasAttribute(options.Tables.ScheduledJob));

			db.RegisterTable(scheduledJobEntityType);

			if (createMissingTables)
			{
				db.InitSchema();
			}
		}
	}
}
