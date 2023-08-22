using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;

namespace Tigerspike.Solv.Core.DynamoDb
{
	/// <summary>
	/// The DynamoDB extensions
	/// </summary>
	public static class DynamoDbExtensions
	{
		/// <summary>
		/// The method to instantiate the DynamoDB for the service url
		/// </summary>
		/// <param name="serviceUrl"></param>
		/// <returns></returns>
		public static IAmazonDynamoDB CreateAmazonDynamoDb(string serviceUrl) => new AmazonDynamoDBClient(CreateAmazonDynamoConfig(serviceUrl));

		/// <summary>
		/// The method to instantiate the DynamoDB configuration
		/// </summary>
		/// <param name="serviceUrl"></param>
		/// <returns></returns>
		public static AmazonDynamoDBConfig CreateAmazonDynamoConfig(string serviceUrl)
		{
			var clientConfig = new AmazonDynamoDBConfig
			{
				RegionEndpoint = RegionEndpoint.EUWest1,
				RetryMode = RequestRetryMode.Standard,
				MaxErrorRetry = 2
			};

			if (!string.IsNullOrWhiteSpace(serviceUrl))
			{
				clientConfig.ServiceURL = serviceUrl;
			}

			return clientConfig;
		}
	}
}
