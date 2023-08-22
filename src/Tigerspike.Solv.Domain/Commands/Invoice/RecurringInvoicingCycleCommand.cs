using MassTransit.Scheduling;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Invoice
{
	/// <summary>
	/// Recurring invoicing cycle command
	/// </summary>
	public class RecurringInvoicingCycleCommand : IRecurringCommand
	{

		/// <summary>
		/// Empty constructor, so MassTransit can deserialize the message
		/// </summary>
		protected RecurringInvoicingCycleCommand()
		{

		}

		public RecurringInvoicingCycleCommand(string cronExpression)
		{
			Schedule = new InvoicingCycleWeeklySchedule(cronExpression);
		}

		public RecurringSchedule Schedule { get; }
	}
}