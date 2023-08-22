using Amazon;
using Amazon.DynamoDBv2;
using ServiceStack;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Models;

namespace Tigerspike.Solv.Services.Fraud.Configuration
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
			var ruleEntityType = typeof(Rule);
			ruleEntityType.AddAttributes(new AliasAttribute(options.Tables.Rule));

			var ticketEntityType = typeof(Ticket);
			ticketEntityType.AddAttributes(new AliasAttribute(options.Tables.Ticket));

			var ticketDetectionEntityType = typeof(TicketDetection);
			ticketDetectionEntityType.AddAttributes(new AliasAttribute(options.Tables.TicketDetection));

			db.RegisterTable(ruleEntityType);
			db.RegisterTable(ticketEntityType);
			db.RegisterTable(ticketDetectionEntityType);

			if (createMissingTables)
			{
				db.InitSchema();
			}
		}
	}
}