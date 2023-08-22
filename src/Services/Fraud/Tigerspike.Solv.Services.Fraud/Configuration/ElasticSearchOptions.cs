namespace Tigerspike.Solv.Services.Fraud.Configuration
{
	public class ElasticSearchOptions
	{
		public const string SectionName = "ElasticSearch";

		public string Uri { get; set; }

		public string DefaultIndex { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }
	}
}