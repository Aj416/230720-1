using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Enums;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Events.Ticket
{
	public class TicketWebHookEventHandler :
		INotificationHandler<TicketCreatedEvent>,
		INotificationHandler<TicketAcceptedEvent>,
		INotificationHandler<TicketAbandonedEvent>,
		INotificationHandler<TicketSolvedEvent>,
		INotificationHandler<TicketReopenedEvent>,
		INotificationHandler<TicketClosedEvent>,
		INotificationHandler<TicketEscalatedEvent>,
		INotificationHandler<TicketTagsChangedEvent>
	{
		private readonly ITicketRepository _ticketRepository;
		private readonly IWebHookService _webHookService;
		private readonly ITimestampService _timestampService;
		private readonly IChatService _chatService;

		public TicketWebHookEventHandler(
			IChatService chatService,
			IWebHookService webHookService,
			ITicketRepository ticketRepository,
			ITimestampService timestampService)
		{
			_chatService = chatService;
			_webHookService = webHookService;
			_ticketRepository = ticketRepository;
			_timestampService = timestampService;
		}

		public async Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
		{
			await PublishEventToWebHook(notification.TicketId, notification.BrandId, notification.ReferenceId,
				notification.IsPractice, notification.SourceName, TicketStatusEnum.New, TicketStatusEnum.New);
		}

		public async Task Handle(TicketAcceptedEvent notification, CancellationToken cancellationToken)
		{
			await PublishEventToWebHook(notification.TicketId, notification.BrandId, notification.ReferenceId,
				notification.IsPractice, notification.SourceName, TicketStatusEnum.New, TicketStatusEnum.Assigned);
		}

		public async Task Handle(TicketAbandonedEvent notification, CancellationToken cancellationToken)
		{
			await PublishEventToWebHook(notification.TicketId, notification.BrandId, notification.ReferenceId,
				notification.IsPractice, notification.SourceName, TicketStatusEnum.Assigned, TicketStatusEnum.New);
		}

		public async Task Handle(TicketSolvedEvent notification, CancellationToken cancellationToken)
		{
			await PublishEventToWebHook(notification.TicketId, notification.BrandId, notification.ReferenceId,
				notification.IsPractice, notification.SourceName, TicketStatusEnum.Assigned, TicketStatusEnum.Solved);
		}

		public async Task Handle(TicketReopenedEvent notification, CancellationToken cancellationToken)
		{
			await PublishEventToWebHook(notification.TicketId, notification.BrandId, notification.ReferenceId,
				notification.IsPractice, notification.SourceName, TicketStatusEnum.Solved, TicketStatusEnum.Assigned);
		}

		public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken)
		{
			await PublishEventToWebHook(notification.TicketId, notification.BrandId, notification.ReferenceId,
				notification.IsPractice, notification.SourceName, TicketStatusEnum.Solved, TicketStatusEnum.Closed);
		}

		public async Task Handle(TicketEscalatedEvent notification, CancellationToken cancellationToken)
		{
			await PublishEventToWebHook(notification.TicketId, notification.BrandId, notification.ReferenceId,
				notification.IsPractice, notification.Source, notification.FromStatus, TicketStatusEnum.Escalated);
		}

		public async Task Handle(TicketTagsChangedEvent notification, CancellationToken cancellationToken)
		{
			var payload = new Dictionary<string, object>
					{
						{"TicketId", notification.TicketId},
						{"ReferenceId", notification.ReferenceId},
						{"PreviousTags", notification.PreviousTags},
						{"CurrentTags", notification.CurrentTags},
						{"RemovedTags", notification.PreviousTags.Except(notification.CurrentTags)},
						{"AddedTags", notification.CurrentTags.Except(notification.PreviousTags)},
						{"EventId", Guid.NewGuid()},
						{"EventType", WebHookEventTypes.TicketTagsChangedEvent.ToString()},
						{"Timestamp", _timestampService.GetUtcTimestamp()}
					};

			await _webHookService.Publish(notification.BrandId, WebHookEventTypes.TicketTagsChangedEvent, payload);
		}

		private async Task PublishEventToWebHook(Guid ticketId, Guid brandId, string referenceId, bool isPractice,
			string source,
			TicketStatusEnum fromStatus, TicketStatusEnum toStatus)
		{
			// ignore webhooks for practice tickets.
			if (isPractice)
			{
				return;
			}

			var payload = new Dictionary<string, object>
					{
						{"TicketId", ticketId},
						{"ReferenceId", referenceId},
						{"Source", source},
						{"FromStatus", fromStatus},
						{"ToStatus", toStatus},
						{"EventId", Guid.NewGuid()},
						{"EventType", WebHookEventTypes.TicketStatusChangedEvent.ToString()},
						{"Timestamp", _timestampService.GetUtcTimestamp()}
					};

			var transcriptStatuses = new[] { TicketStatusEnum.Closed, TicketStatusEnum.Escalated };
			if (toStatus.IsIn(transcriptStatuses))
			{
				// provide transcript when ticket is close or escalated
				payload.Add("Transcript", await _chatService.GetTranscript(ticketId));
			}

			if (toStatus == TicketStatusEnum.Escalated)
			{
				// for escalation add additional data
				payload.Add("EscalationReason", (await _ticketRepository.FindAsync(ticketId)).EscalationReason);
			}

			await _webHookService.Publish(brandId, WebHookEventTypes.TicketStatusChangedEvent, payload);
		}
	}
}