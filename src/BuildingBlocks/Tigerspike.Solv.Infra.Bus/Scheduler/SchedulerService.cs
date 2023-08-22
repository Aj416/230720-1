using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.Exceptions;
using Tigerspike.Solv.Infra.Bus.Model;
using Tigerspike.Solv.Infra.Bus.Repositories;

namespace Tigerspike.Solv.Infra.Bus.Scheduler
{
	public sealed class SchedulerService : ISchedulerService
	{
		private readonly IBus _bus;
		private readonly IScheduledJobRepository _scheduledJobRepository;
		private readonly ILogger<SchedulerService> _logger;
		private readonly BusOptions _busOptions;
		private IMessageScheduler _messageScheduler;

		public SchedulerService(
			IBus bus,
			IScheduledJobRepository scheduledJobRepository,
			IMessageScheduler messageScheduler,
			IOptions<BusOptions> busOptions,
			ILogger<SchedulerService> logger)
		{
			_bus = bus;
			_scheduledJobRepository = scheduledJobRepository;
			_messageScheduler = messageScheduler;
			_logger = logger;
			_busOptions = busOptions.Value;
		}

		/// <inheritdoc/>
		public async Task SetRecurringJob<T>(T command, bool state) where T : IRecurringCommand
		{
			var schedule = command.Schedule;
			var destination = _bus.GetQueueUri(_busOptions.Queues.Schedule);
			var schedulerEndpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Quartz}"));

			if (state)
			{
				_logger.LogInformation("Turning on recurring job {job} with schedule: {schedule}", typeof(T).Name,
					command.Schedule.Description);
				await schedulerEndpoint.ScheduleRecurringSend(destination, schedule, command);
			}
			else
			{
				_logger.LogInformation("Turning off recurring job {job}", typeof(T).Name);
				await _bus.CancelScheduledRecurringSend(schedule.ScheduleId, schedule.ScheduleGroup);
			}
		}

		/// <inheritdoc/>
		public Task ScheduleJob<T>(T command, DateTime scheduleTimestamp) where T: class, IScheduledJob
		{
			if (EndpointConvention.TryGetDestinationAddress<T>(out var destination))
			{
				return ScheduleJob(command, scheduleTimestamp, destination);
			}
			else
			{
				throw new BusConfigurationException($"Unknown destination for message of type {typeof(T)}");
			}
		}

		public async Task ScheduleJob<T>(T command, DateTime scheduleTimestamp, Uri destination) where T : class, IScheduledJob
		{
			var scheduledMessage = await _messageScheduler.ScheduleSend(destination, scheduleTimestamp, command);
			_logger.LogDebug("Schedule command {cmd} on {timestamp}", typeof(T).Name, scheduleTimestamp);

			var job = new ScheduledJob(command.JobId, scheduledMessage.TokenId, scheduleTimestamp);
			_scheduledJobRepository.PutItem(job);
		}

		/// <inheritdoc/>
		public Task CancelScheduledJob<T>(T command) where T : class, IScheduledJob
		{
			if (EndpointConvention.TryGetDestinationAddress<T>(out var destination))
			{
				return CancelScheduledJob(command, destination);
			}
			else
			{
				throw new BusConfigurationException($"Unknown destination for message of type {typeof(T)}");
			}
		}

		public async Task CancelScheduledJob<T>(T command, Uri destination) where T : class, IScheduledJob
		{
			var job = _scheduledJobRepository.GetItem(command.JobId);
			if (job != null)
			{
				await _messageScheduler.CancelScheduledSend(destination, job.Token);
				_logger.LogDebug("Cancelled scheduled command {jobId}", command.JobId);
			}
		}
	}
}