using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Infra.Bus.Scheduler
{
	public interface ISchedulerService
	{

		/// <summary>
		/// Sets the recurring job command to desired state (on/off)
		/// </summary>
		/// <param name="command">Recurring job command</param>
		/// <param name="state">True to set job on, false to turn it off</param>
		Task SetRecurringJob<T>(T command, bool state) where T : IRecurringCommand;

		/// <summary>
		/// Schedules delivery of command on specified timestamp
		/// </summary>
		Task ScheduleJob<T>(T command, DateTime scheduleTimestamp) where T : class, IScheduledJob;

		/// <summary>
		/// Schedules delivery of command on specified timestamp
		/// </summary>
		Task ScheduleJob<T>(T command, DateTime scheduleTimestamp, Uri destination) where T : class, IScheduledJob;

		/// <summary>
		/// Cancel scheduled command
		/// </summary>
		Task CancelScheduledJob<T>(T command) where T : class, IScheduledJob;

		/// <summary>
		/// Cancel scheduled command
		/// </summary>
		Task CancelScheduledJob<T>(T command, Uri destination) where T : class, IScheduledJob;

	}
}