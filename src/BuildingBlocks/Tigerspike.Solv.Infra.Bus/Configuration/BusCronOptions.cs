namespace Tigerspike.Solv.Infra.Bus.Configuration
{
	public class BusCronOptions
	{
		public const string SectionName = BusOptions.SectionName + ".Cron";

		public string InvoicingCycleWeeklySchedule { get; set; }
	}
}