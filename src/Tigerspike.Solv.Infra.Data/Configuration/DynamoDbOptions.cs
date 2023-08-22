namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class DynamoDbOptions
	{
		public const string SectionName = "DynamoDb";

		public string AccessKey { get; set; }

		public string SecretKey { get; set; }

		public string ServiceUrl { get; set; }

		public DynamoDbTableOptions Tables { get; set; }
	}

	public class DynamoDbTableOptions
	{
		public const string SectionName = DynamoDbOptions.SectionName + ".Tables";
		public string ScheduledJob { get; set; }
	}
}