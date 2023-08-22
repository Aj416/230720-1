namespace Tigerspike.Solv.Services.Invoicing.Configuration
{
	/// <summary>
	/// For fetching DynamoDb configuration.
	/// </summary>
	public class DynamoDbOptions
	{
		/// <summary>
		/// Section name to be referred in app settings.
		/// </summary>
		public const string SectionName = "DynamoDb";

		/// <summary>
		/// Access key for AWS account.
		/// </summary>
		public string AccessKey { get; set; }

		/// <summary>
		/// Secret key for AWS account.
		/// </summary>
		public string SecretKey { get; set; }

		/// <summary>
		/// Service Url for AWS account.
		/// </summary>
		public string ServiceUrl { get; set; }

		/// <summary>
		/// Tables to be created in DynamoDb.
		/// </summary>
		public DynamoDbTableOptions Tables { get; set; }
	}

	/// <summary>
	/// For fetching DynamoDb table configuration.
	/// </summary>
	public class DynamoDbTableOptions
	{
		/// <summary>
		/// Section name to be referred in app settings.
		/// </summary>
		public const string SectionName = DynamoDbOptions.SectionName + ".Tables";

		/// <summary>
		/// Subscription Table.
		/// </summary>
		public string ScheduledJob { get; set; }
	}
}
