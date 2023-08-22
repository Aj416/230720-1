using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Redis;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Infra.Bus.Scheduler;
using Tigerspike.Solv.Infra.Data.Interfaces;
using static Tigerspike.Solv.Core.Constants.CacheKeys;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	public class TicketEventHandler :
		INotificationHandler<TicketCreatedEvent>,
		INotificationHandler<TicketComplexitySetEvent>,
		INotificationHandler<TicketSolvedEvent>,
		INotificationHandler<TicketReopenedEvent>,
		INotificationHandler<TicketClosedEvent>,
		INotificationHandler<TicketAcceptedEvent>,
		INotificationHandler<TicketReservedEvent>,
		INotificationHandler<TicketAbandonedEvent>,
		INotificationHandler<TicketChaserEmailSentEvent>,
		INotificationHandler<TicketSposLeadSetEvent>
	{
		private readonly IMediatorHandler _mediator;
		private readonly ISchedulerService _schedulerService;
		private readonly ITimestampService _timestampService;
		private readonly ITicketEscalationConfigRepository _ticketEscalationConfigRepository;
		private readonly IBrandRepository _brandRepository;
		private readonly IRedisClientsManager _redisClientsManager;

		public TicketEventHandler(
			ITicketEscalationConfigRepository ticketEscalationConfigRepository,
			IBrandRepository brandRepository,
			IRedisClientsManager redisClientsManager,
			ISchedulerService schedulerService,
			IMediatorHandler mediator,
			ITimestampService timestampService)
		{
			_ticketEscalationConfigRepository = ticketEscalationConfigRepository;
			_brandRepository = brandRepository;
			_schedulerService = schedulerService;
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			_timestampService = timestampService;
			_redisClientsManager = redisClientsManager;
		}

		public async Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.TransportType.IsIn(TicketTransportType.Chat, TicketTransportType.Email))
			{
				await _mediator.SendCommand(new SendTicketCreatedEmail(notification.TicketId, notification.Question, notification.CustomerId, notification.BrandId, notification.Culture));
			}
		}

		public async Task Handle(TicketComplexitySetEvent notification, CancellationToken cancellationToken)
		{
			if (notification.AdvocateId != null)
			{
				using (var client = _redisClientsManager.GetClient())
				{
					// invalidate statistics changed due to hide fraud ticket for the advocate.
					client.Remove(CurrentWeekStatisticsPeriodKey(notification.AdvocateId.Value));
					client.Remove(AllTimeStatisticsPeriodKey(notification.AdvocateId.Value));
				}
			}

			if (notification.IsPractice)
			{
				await _mediator.SendCommand(new FinishAdvocatePracticeCommand(notification.TicketId));
			}

			// TODO: Send a command to update the search index.
		}

		public async Task Handle(TicketSolvedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.TransportType == TicketTransportType.Chat)
			{
				var notifications = await _brandRepository.GetNotificationConfig(notification.BrandId, BrandNotificationType.TicketSolved_Chat);
				foreach (var notificationConfig in notifications)
				{
					var reminderTimestamp = notification.Timestamp.AddSeconds(notificationConfig.DeliverAfterSeconds);
					var cmd = new SendCloseTicketReminderCommand(notification.TicketId, notificationConfig.Id, notification.AdvocateId, notificationConfig.Subject, notificationConfig.Header, notificationConfig.Body);
					await _schedulerService.ScheduleJob(cmd, reminderTimestamp);
				}
			}

			var deliveryTimestamp = notification.Timestamp.AddMinutes(notification.WaitMinutesToClose);
			await _schedulerService.ScheduleJob(new CloseTicketWhenNoResponseCommand(notification.TicketId),
				deliveryTimestamp);
		}

		public async Task Handle(TicketAbandonedEvent notification, CancellationToken cancellationToken)
		{
			// clear tags on tickets, when abandoned
			await _mediator.SendCommand(new SetTicketTagsCommand(notification.TicketId, new Guid[] { }, notification.Level));
		}

		public async Task Handle(TicketReopenedEvent notification, CancellationToken cancellationToken)
		{
			await _schedulerService.CancelScheduledJob(new CancelTicketReservationCommand(notification.TicketId, null));
			await CancelCloseTicketReminders(notification.BrandId, notification.TicketId);

			if (notification.AdvocateBlocked)
			{
				// If the advocate is blocked when the ticket is reopened, we need to force abandoning the ticket.
				var reason = await _brandRepository.Queryable()
					.Where(x => x.Id == notification.BrandId)
					.SelectMany(s => s.AbandonReasons.Where(a => a.IsBlockedAdvocate).Select(s => s.Id)).SingleOrDefaultAsync();

				await _mediator.SendCommand(new AbandonTicketCommand(notification.TicketId, new Guid[] { reason }));
			}
		}

		public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.AdvocateId != null)
			{
				using (var client = _redisClientsManager.GetClient())
				{
					// invalidate statistics changed due to closing the ticket for the advocate
					client.Remove(CurrentWeekStatisticsPeriodKey(notification.AdvocateId.Value));
					client.Remove(AllTimeStatisticsPeriodKey(notification.AdvocateId.Value));
				}
			}

			await CancelCloseTicketReminders(notification.BrandId, notification.TicketId);
		}

		public async Task Handle(TicketAcceptedEvent notification, CancellationToken cancellationToken)
		{
			await _schedulerService.CancelScheduledJob(new CancelTicketReservationCommand(notification.TicketId, null));
			await _schedulerService.CancelScheduledJob(new CloseTicketWhenNoResponseCommand(notification.TicketId));
		}

		public async Task Handle(TicketReservedEvent notification, CancellationToken cancellationToken)
		{
			await _schedulerService.ScheduleJob(
				new CancelTicketReservationCommand(notification.TicketId, notification.AdvocateId),
				notification.ReservationExpiryDate);
		}

		public Task Handle(TicketChaserEmailSentEvent notification, CancellationToken cancellationToken) =>
			_mediator.SendCommand(new IncreaseTicketChaserEmailsCommand(notification.TicketId));

		public async Task Handle(TicketSposLeadSetEvent notification, CancellationToken cancellationToken) =>
				await _mediator.SendCommand(new SendTicketSposEmail(notification.TicketId, notification.CustomerId, notification.BrandId, notification.Notify));


		private async Task CancelCloseTicketReminders(Guid brandId, Guid ticketId)
		{
			var notifications = await _brandRepository.GetNotificationConfig(brandId, BrandNotificationType.TicketSolved_Chat, activeOnly: false);
			foreach (var notificationConfig in notifications)
			{
				var cmd = new SendCloseTicketReminderCommand(ticketId, notificationConfig.Id);
				await _schedulerService.CancelScheduledJob(cmd);
			}
		}
	}
}