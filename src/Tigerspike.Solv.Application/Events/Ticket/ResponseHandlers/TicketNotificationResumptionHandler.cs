using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Brand;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Infra.Data.Models;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	public class TicketNotificationResumptionHandler :
		INotificationHandler<TicketChatResumedEvent>,
		INotificationHandler<TicketReopenedEvent>,
		INotificationHandler<TicketAbandonedEvent>,
		INotificationHandler<TicketEscalatedEvent>,
		INotificationHandler<TicketSolvedEvent>,
		INotificationHandler<TicketClosedEvent>
	{

		private readonly ITicketAutoResponseService _ticketAutoResponseService;
		private readonly ICachedTicketRepository _cachedTicketRepository;

		public TicketNotificationResumptionHandler(
			ITicketAutoResponseService ticketAutoResponseService,
			ICachedTicketRepository cachedTicketRepository
			)
		{
			_ticketAutoResponseService = ticketAutoResponseService;
			_cachedTicketRepository = cachedTicketRepository;
		}

		public async Task Handle(TicketChatResumedEvent notification, CancellationToken cancellationToken)
		{
			// first cancel previous chat resume flow
			await CancelNotificationResumptionFlow(notification.BrandId, notification.TicketId);

			// now schedule new flow
			var model = await GetTemplateModel(notification.BrandId, notification.AdvocateFirstName, notification.CustomerFirstName);
			var responseTypes = GetNotificationResumptionTicketResponseType(notification.Status, NotificationResumptionState.CustomerResumed);
			if (responseTypes != null)
			{
				await _ticketAutoResponseService.SendResponses(notification.BrandId, notification.TicketId, responseTypes, model: model);
			}
		}

		public async Task Handle(TicketReopenedEvent notification, CancellationToken cancellationToken)
		{
			var model = await GetTemplateModel(notification.BrandId, notification.AdvocateFirstName, notification.CustomerFirstName);
			var responseTypes = GetNotificationResumptionTicketResponseType(TicketStatusEnum.Assigned, notification.NotificationResumptionState, true);
			if (responseTypes != null)
			{
				await _ticketAutoResponseService.SendResponses(notification.BrandId, notification.TicketId, responseTypes, model: model);
			}
		}

		public async Task Handle(TicketAbandonedEvent notification, CancellationToken cancellationToken) => await CancelNotificationResumptionFlow(notification.BrandId, notification.TicketId);

		public async Task Handle(TicketEscalatedEvent notification, CancellationToken cancellationToken) => await CancelNotificationResumptionFlow(notification.BrandId, notification.TicketId);

		public async Task Handle(TicketSolvedEvent notification, CancellationToken cancellationToken) => await CancelNotificationResumptionFlow(notification.BrandId, notification.TicketId);

		public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken) => await CancelNotificationResumptionFlow(notification.BrandId, notification.TicketId);

		private async Task<BrandResponseTemplateModel> GetTemplateModel(Guid brandId, string advocateFirstName, string customerFirstName)
		{
			var nextActionDelay = await GetNextActionDelay(brandId);
			return new BrandResponseTemplateModel(advocateFirstName, customerFirstName, nextActionDelay);
		}

		private async Task<TimeSpan> GetNextActionDelay(Guid brandId)
		{
			var nextActions = await _ticketAutoResponseService.GetResponses(brandId, new[] { BrandAdvocateResponseType.NotificationResumptionTicketAbandoned });
			return nextActions.Select(x => TimeSpan.FromSeconds(x.DelayInSeconds ?? 0)).FirstOrDefault();
		}

		private async Task CancelNotificationResumptionFlow(Guid brandId, Guid ticketId)
		{
			await _ticketAutoResponseService.CancelResponses(brandId, ticketId, new[] { BrandAdvocateResponseType.NotificationResumptionTicketAbandoned });
			_cachedTicketRepository.SetNotificationResumptionFlowContext(new NotificationResumptionFlowContext(ticketId, brandId));
		}


		private BrandAdvocateResponseType[] GetNotificationResumptionTicketResponseType(TicketStatusEnum status, NotificationResumptionState state, bool? reopened = null) =>
			(state, status, reopened) switch
			{
				(NotificationResumptionState.CustomerResumed, TicketStatusEnum.Assigned, true) => new[] { BrandAdvocateResponseType.NotificationResumptionTicketReopened, BrandAdvocateResponseType.NotificationResumptionTicketAbandoned },
				(NotificationResumptionState.CustomerResumed, TicketStatusEnum.Assigned, _) => new[] { BrandAdvocateResponseType.NotificationResumptionTicketInProgress, BrandAdvocateResponseType.NotificationResumptionTicketAbandoned },
				(NotificationResumptionState.CustomerResumed, TicketStatusEnum.Solved, _) => new[] { BrandAdvocateResponseType.NotificationResumptionTicketSolved },
				_ => null,
			};
	}
}