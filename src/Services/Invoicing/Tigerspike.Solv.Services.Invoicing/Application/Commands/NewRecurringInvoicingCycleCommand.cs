using MassTransit.Scheduling;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Services.Invoicing.Application.Commands
{
	public class NewRecurringInvoicingCycleCommand : IRecurringCommand
	{

		/// <summary>
		/// Empty constructor, so MassTransit can deserialize the message
		/// </summary>
		protected NewRecurringInvoicingCycleCommand()
		{

		}

		public NewRecurringInvoicingCycleCommand(string cronExpression) => Schedule = new InvoicingCycleWeeklySchedule(cronExpression);

		public RecurringSchedule Schedule { get; }
	}
}
