using System;
using MassTransit.Scheduling;

namespace Tigerspike.Solv.Core.Bus
{
	public class InvoicingCycleWeeklySchedule : DefaultRecurringSchedule
	{
		/// <summary>
		/// Creates a schedule for MassTransit
		/// </summary>
		/// <param name="cronExpression">https://www.freeformatter.com/cron-expression-generator-quartz.html</param>
		public InvoicingCycleWeeklySchedule(string cronExpression)
		{
			MisfirePolicy = MissedEventPolicy.Skip;
			TimeZoneId = TimeZoneInfo.Utc.Id;

			if (string.IsNullOrWhiteSpace(cronExpression) == false)
			{
				CronExpression = cronExpression;
				Description = CronExpressionDescriptor.ExpressionDescriptor.GetDescription(cronExpression);
			}
		}
	}
}