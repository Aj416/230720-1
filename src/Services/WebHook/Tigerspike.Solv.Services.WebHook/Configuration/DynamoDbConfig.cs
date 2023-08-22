using Amazon;
using Amazon.DynamoDBv2;
using ServiceStack;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Services.WebHook.Domain;

namespace Tigerspike.Solv.Services.WebHook.Configuration
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
			var ruleEntityType = typeof(Subscription);
			ruleEntityType.AddAttributes(new AliasAttribute(options.Tables.Subscription));

			db.RegisterTable(ruleEntityType);

			if (createMissingTables)
			{
				db.InitSchema();
			}
		}
	}
}