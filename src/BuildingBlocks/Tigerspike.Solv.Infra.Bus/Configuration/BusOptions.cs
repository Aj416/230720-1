using System.Collections.Generic;

namespace Tigerspike.Solv.Infra.Bus.Configuration
{
	public class BusOptions
	{
		public const string SectionName = "Bus";

		public BusOptions()
		{
			Sqs = new BusServiceOptions();
			Sns = new BusServiceOptions();
			Queues = new BusQueueOptions();
			Cron = new BusCronOptions();
		}

		public string Region { get; set; }

		public string Protocol { get; set; }

		public string Transport { get; set; }

		public string AccessKey { get; set; }

		public string SecretKey { get; set; }

		public bool IncludeScheduler { get; set; }

		public bool UseServiceUrl { get; set; }

		public BusServiceOptions Sqs { get; set; }

		public BusServiceOptions Sns { get; set; }

		public BusQueueOptions Queues { get; set; }

		public BusCronOptions Cron { get; set; }
	}
}