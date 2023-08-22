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
	public class TicketReturningCustomerHandler :
		INotificationHandler<TicketMarkedAsReturningCustomerEvent>,
		INotificationHandler<TicketReopenedEvent>,
		INotificationHandler<TicketAbandonedEvent>,
		INotificationHandler<TicketEscalatedEvent>,
		INotificationHandler<TicketSolvedEvent>,
		INotificationHandler<TicketClosedEvent>
	{

		private readonly ITicketAutoResponseService _ticketAutoResponseService;
		private readonly ICachedTicketRepository _cachedTicketRepository;

		public TicketReturningCustomerHandler(
			ITicketAutoResponseService ticketAutoResponseService,
			ICachedTicketRepository cachedTicketRepository
			)
		{
			_ticketAutoResponseService = ticketAutoResponseService;
			_cachedTicketRepository = cachedTicketRepository;
		}

		public async Task Handle(TicketMarkedAsReturningCustomerEvent notification, CancellationToken cancellationToken)
		{
			// first cancel previous Returning Customer Flow
			await CancelReturningCustomerFlow(notification.BrandId, notification.TicketId);

			// now schedule new flow
			var model = await GetTemplateModel(notification.BrandId, notification.AdvocateFirstName, notification.CustomerFirstName);
			var responseTypes = GetReturningCustomerTicketResponseType(notification.Status, ReturningCustomerState.CustomerReturned);
			if (responseTypes != null)
			{
				await _ticketAutoResponseService.SendResponses(notification.BrandId, notification.TicketId, responseTypes, model: model);
			}
		}
		public async Task Handle(TicketReopenedEvent notification, CancellationToken cancellationToken)
		{
			var model = await GetTemplateModel(notification.BrandId, notification.AdvocateFirstName, notification.CustomerFirstName);
			var responseTypes = GetReturningCustomerTicketResponseType(TicketStatusEnum.Assigned, notification.ReturningCustomerState, true);
			if (responseTypes != null)
			{
				await _ticketAutoResponseService.SendResponses(notification.BrandId, notification.TicketId, responseTypes, model: model);
			}
		}

		public async Task Handle(TicketAbandonedEvent notification, CancellationToken cancellationToken) => await CancelReturningCustomerFlow(notification.BrandId, notification.TicketId);

		public async Task Handle(TicketEscalatedEvent notification, CancellationToken cancellationToken) => await CancelReturningCustomerFlow(notification.BrandId, notification.TicketId);

		public async Task Handle(TicketSolvedEvent notification, CancellationToken cancellationToken) => await CancelReturningCustomerFlow(notification.BrandId, notification.TicketId);

		public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken) => await CancelReturningCustomerFlow(notification.BrandId, notification.TicketId);

		private async Task<BrandResponseTemplateModel> GetTemplateModel(Guid brandId, string advocateFirstName, string customerFirstName)
		{
			var nextActionDelay = await GetNextActionDelay(brandId);
			return new BrandResponseTemplateModel(advocateFirstName, customerFirstName, nextActionDelay);
		}

		private async Task<TimeSpan> GetNextActionDelay(Guid brandId)
		{
			var nextActions = await _ticketAutoResponseService.GetResponses(brandId, new[] { BrandAdvocateResponseType.ReturningCustomerTicketAbandoned });
			return nextActions.Select(x => TimeSpan.FromSeconds(x.DelayInSeconds ?? 0)).FirstOrDefault();
		}

		private async Task CancelReturningCustomerFlow(Guid brandId, Guid ticketId)
		{
			await _ticketAutoResponseService.CancelResponses(brandId, ticketId, new[] { BrandAdvocateResponseType.ReturningCustomerTicketAbandoned });
			_cachedTicketRepository.SetReturningCustomerFlowContext(new ReturningCustomerFlowContext(ticketId, brandId));
		}

		private BrandAdvocateResponseType[] GetReturningCustomerTicketResponseType(TicketStatusEnum status, ReturningCustomerState state, bool? reopened = null) =>
			(state, status, reopened) switch
			{
				(ReturningCustomerState.CustomerReturned, TicketStatusEnum.New, _) => new[] { BrandAdvocateResponseType.ReturningCustomerTicketOpen },
				(ReturningCustomerState.CustomerReturned, TicketStatusEnum.Assigned, true) => new[] { BrandAdvocateResponseType.ReturningCustomerTicketReopened, BrandAdvocateResponseType.ReturningCustomerTicketAbandoned },
				(ReturningCustomerState.CustomerReturned, TicketStatusEnum.Assigned, _) => new[] { BrandAdvocateResponseType.ReturningCustomerTicketInProgress, BrandAdvocateResponseType.ReturningCustomerTicketAbandoned },
				(ReturningCustomerState.CustomerReturned, TicketStatusEnum.Solved, _) => new[] { BrandAdvocateResponseType.ReturningCustomerTicketSolved },
				_ => null,
			};
	}
}