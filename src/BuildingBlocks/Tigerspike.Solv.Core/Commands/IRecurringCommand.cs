using MassTransit.Scheduling;

namespace Tigerspike.Solv.Core.Commands
{
	public interface IRecurringCommand
	{
			RecurringSchedule Schedule { get; }
	}
}