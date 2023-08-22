namespace Tigerspike.Solv.Infra.Bus.Configuration
{
	public class BusQueueOptions
	{
		public const string SectionName = BusOptions.SectionName + ".Queues";

		public string Advocate { get; set; }

		public string Ticket { get; set; }
		public string Export { get; set; }
		public string Import { get; set; }

		public string WebHook { get; set; }

		public string Schedule { get; set; }

		public string Quartz { get; set; }

		public string Invoicing { get; set; }

		public string Email { get; set; }

		public string Notification { get; set; }

		public string Chat { get; set; }

		public string Fraud { get; set; }

		public string Brand { get; set; }

		public string Payment { get; set; }

		public string Workflow { get; set; }

		public string IdentityVerification { get; set; }
		
		public string NewInvoicing { get; set; }
	}
}