using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Core.DynamoDb;
using Tigerspike.Solv.Infra.Bus.Configuration;
using WorkflowCore.Models;

namespace Tigerspike.Solv.Application.Configuration
{
	/// <summary>
	/// The DynamoDB configuration class
	/// </summary>
	public static class DynamoDbConfig
	{
		/// <summary>
		/// Method to setup DynamoDB persistance for the workflows
		/// </summary>
		/// <param name="cfg"></param>
		/// <param name="dynamoOptions"></param>
		/// <param name="busOptions"></param>
		/// <returns></returns>
		public static WorkflowOptions Setup(this WorkflowOptions cfg, DynamoDbOptions dynamoOptions, BusOptions busOptions)
		{
			var dynamoConfig = DynamoDbExtensions.CreateAmazonDynamoConfig(dynamoOptions.ServiceUrl);

			cfg.UseAwsDynamoPersistence(FallbackCredentialsFactory.GetCredentials(), dynamoConfig, dynamoOptions.Tables.WorkflowTablePrefix);
			cfg.UseAwsDynamoLocking(FallbackCredentialsFactory.GetCredentials(), dynamoConfig, dynamoOptions.Tables.WorkflowLocks);
			cfg.UseAwsSimpleQueueService(FallbackCredentialsFactory.GetCredentials(), CreateSQSConfig(busOptions), busOptions.Queues.Workflow);
			return cfg;
		}

		/// <summary>
		/// Method to setup the SQS config
		/// </summary>
		/// <param name="busOptions"></param>
		/// <returns></returns>
		private static AmazonSQSConfig CreateSQSConfig(BusOptions busOptions)
		{
			var config = new AmazonSQSConfig()
			{
				RegionEndpoint = RegionEndpoint.EUWest1
			};

			if (!string.IsNullOrWhiteSpace(busOptions.Sqs.ServiceUrl))
			{
				config.ServiceURL = busOptions.Sqs.ServiceUrl;
			}

			return config;
		}
	}
}
