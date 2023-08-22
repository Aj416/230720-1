using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.SignalR;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using TicketStatusChangedNotificationModel = Tigerspike.Solv.Application.Models.TicketStatusChangedNotificationModel;

namespace Tigerspike.Solv.Chat.Application.EventHandlers
{
	public class SendNotificationWhenTicketStatusChangedEventhandler :
		INotificationHandler<TicketEscalatedEvent>,
		INotificationHandler<TicketAbandonedEvent>,
		INotificationHandler<TicketReopenedEvent>,
		INotificationHandler<TicketSolvedEvent>,
		INotificationHandler<TicketClosedEvent>,
		INotificationHandler<TicketAcceptedEvent>,
		INotificationHandler<TicketReservedEvent>,
		INotificationHandler<TicketRejectedEvent>
	{
		private readonly IHubContext<ChatHub> _chatHub;

		public SendNotificationWhenTicketStatusChangedEventhandler(IHubContext<ChatHub> chatHub) => _chatHub = chatHub;
		public async Task Handle(TicketEscalatedEvent notification, CancellationToken cancellationToken) => await SendStatusChangedNotification(new TicketStatusChangedNotificationModel { TicketId = notification.TicketId, ToStatus = TicketStatusEnum.Escalated, AdvocateId = notification.AdvocateId, AdvocateFirstName = notification.AdvocateFirstName, AdvocateCsat = notification.AdvocateCsat, CustomerId = notification.CustomerId });
		public async Task Handle(TicketAbandonedEvent notification, CancellationToken cancellationToken) => await SendStatusChangedNotification(new TicketStatusChangedNotificationModel { TicketId = notification.TicketId, ToStatus = TicketStatusEnum.New, CustomerId = notification.CustomerId });
		public async Task Handle(TicketReopenedEvent notification, CancellationToken cancellationToken) => await SendStatusChangedNotification(new TicketStatusChangedNotificationModel { TicketId = notification.TicketId, ToStatus = TicketStatusEnum.Assigned, AdvocateId = notification.AdvocateId, AdvocateFirstName = notification.AdvocateFirstName, AdvocateCsat = notification.AdvocateCsat, CustomerId = notification.CustomerId });
		public async Task Handle(TicketSolvedEvent notification, CancellationToken cancellationToken) => await SendStatusChangedNotification(new TicketStatusChangedNotificationModel { TicketId = notification.TicketId, ToStatus = TicketStatusEnum.Solved, AdvocateId = notification.AdvocateId, AdvocateFirstName = notification.AdvocateFirstName, AdvocateCsat = notification.AdvocateCsat, CustomerId = notification.CustomerId });
		public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken) => await SendStatusChangedNotification(new TicketStatusChangedNotificationModel { TicketId = notification.TicketId, ToStatus = TicketStatusEnum.Closed, AdvocateId = notification.AdvocateId, AdvocateFirstName = notification.AdvocateFirstName, AdvocateCsat = notification.AdvocateCsat, CustomerId = notification.CustomerId, ClosedBy = notification.ClosedBy, TagStatus = notification.TagStatus });
		public async Task Handle(TicketAcceptedEvent notification, CancellationToken cancellationToken) => await SendStatusChangedNotification(new TicketStatusChangedNotificationModel { TicketId = notification.TicketId, ToStatus = TicketStatusEnum.Assigned, AdvocateId = notification.AdvocateId, AdvocateFirstName = notification.AdvocateFirstName, AdvocateCsat = notification.AdvocateCsat, CustomerId = notification.CustomerId });
		public async Task Handle(TicketReservedEvent notification, CancellationToken cancellationToken) => await SendStatusChangedNotification(new TicketStatusChangedNotificationModel { TicketId = notification.TicketId, ToStatus = TicketStatusEnum.Reserved, AdvocateId = notification.AdvocateId, AdvocateFirstName = notification.AdvocateFirstName, AdvocateCsat = notification.AdvocateCsat, CustomerId = notification.CustomerId });
		public async Task Handle(TicketRejectedEvent notification, CancellationToken cancellationToken) => await SendStatusChangedNotification(new TicketStatusChangedNotificationModel { TicketId = notification.TicketId, ToStatus = TicketStatusEnum.New, CustomerId = notification.CustomerId });
		private Task SendStatusChangedNotification(TicketStatusChangedNotificationModel ticketStatusChangedNotificationModel)
		{
			if(ticketStatusChangedNotificationModel.ToStatus == TicketStatusEnum.Closed && ticketStatusChangedNotificationModel.ClosedBy != null && ticketStatusChangedNotificationModel.ClosedBy == ClosedBy.EndChat && ticketStatusChangedNotificationModel.TagStatus == null)
			{
				return SendStatusChangedNotificationForEndChat(ticketStatusChangedNotificationModel);
			}

			var userIds = new Guid?[] { ticketStatusChangedNotificationModel.AdvocateId, ticketStatusChangedNotificationModel.CustomerId }
				.Where(x => x.HasValue)
				.Select(s => s.ToString()).ToList();

			return _chatHub.Clients.Users(userIds)
			.SendAsync(ChatHubConstants.TICKET_STATUS_CHANGED, new TicketStatusChangedModel(ticketStatusChangedNotificationModel.TicketId, ticketStatusChangedNotificationModel.ToStatus, ticketStatusChangedNotificationModel.AdvocateId, ticketStatusChangedNotificationModel.AdvocateFirstName, ticketStatusChangedNotificationModel.AdvocateCsat, ticketStatusChangedNotificationModel.ClosedBy));
		}

		private Task SendStatusChangedNotificationForEndChat(TicketStatusChangedNotificationModel ticketStatusChangedNotificationModel)
		{
			var userIds = new Guid?[] { ticketStatusChangedNotificationModel.AdvocateId }
				.Where(x => x.HasValue)
				.Select(s => s.ToString()).ToList();

			var customerIds = new Guid?[] { ticketStatusChangedNotificationModel.CustomerId }
				.Where(x => x.HasValue)
				.Select(s => s.ToString()).ToList();

			var tasks = new List<Task>
			{
				_chatHub.Clients.Users(userIds)
				.SendAsync(ChatHubConstants.TICKET_STATUS_CHANGED, new TicketStatusChangedModel(ticketStatusChangedNotificationModel.TicketId, TicketStatusEnum.EndedByCustomer, ticketStatusChangedNotificationModel.AdvocateId, ticketStatusChangedNotificationModel.AdvocateFirstName, ticketStatusChangedNotificationModel.AdvocateCsat, ticketStatusChangedNotificationModel.ClosedBy)),

				_chatHub.Clients.Users(customerIds)
				.SendAsync(ChatHubConstants.TICKET_STATUS_CHANGED, new TicketStatusChangedModel(ticketStatusChangedNotificationModel.TicketId, ticketStatusChangedNotificationModel.ToStatus, ticketStatusChangedNotificationModel.AdvocateId, ticketStatusChangedNotificationModel.AdvocateFirstName, ticketStatusChangedNotificationModel.AdvocateCsat, ticketStatusChangedNotificationModel.ClosedBy))
			};

			return Task.WhenAll(tasks);
		}
	}
}
