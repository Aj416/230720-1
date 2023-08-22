namespace Tigerspike.Solv.Core.DynamoDb
{
	/// <summary>
	/// The DynamoDB Options
	/// </summary>
	public class DynamoDbOptions
	{
		/// <summary>
		/// The section name of the appsettings
		/// </summary>
		public const string SectionName = "DynamoDb";

		/// <summary>
		/// The AWS access key
		/// </summary>
		public string AccessKey { get; set; }

		/// <summary>
		/// The AWS secret
		/// </summary>
		public string SecretKey { get; set; }

		/// <summary>
		/// The service URL
		/// </summary>
		public string ServiceUrl { get; set; }

		/// <summary>
		/// The tables of the database
		/// </summary>
		public DynamoDbTableOptions Tables { get; set; }
	}

	/// <summary>
	/// The DynamoDB tables
	/// </summary>
	public class DynamoDbTableOptions
	{
		/// <summary>
		/// The section name
		/// </summary>
		public const string SectionName = DynamoDbOptions.SectionName + ".Tables";

		/// <summary>
		/// The conversation
		/// </summary>
		public string Conversation { get; set; }

		/// <summary>
		/// The message
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The message whitelist
		/// </summary>
		public string MessageWhitelist { get; set; }

		/// <summary>
		/// The scheduled job
		/// </summary>
		public string ScheduledJob { get; set; }

		/// <summary>
		/// The table prefix for the workflow tables
		/// </summary>
		public string WorkflowTablePrefix { get; set; }

		/// <summary>
		/// The locks for the workflows
		/// </summary>
		public string WorkflowLocks { get; set; }

		/// <summary>
		/// The chat actions.
		/// </summary>
		public string ChatAction { get; set; }

		/// <summary>
		/// The identity verification identity checks.
		/// </summary>
		public string IdentityVerificationCheck { get; set; }
	}
}
