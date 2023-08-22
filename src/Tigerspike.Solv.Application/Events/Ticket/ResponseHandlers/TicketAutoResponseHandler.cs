using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Brand;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	public class TicketAutoResponseHandler : INotificationHandler<TicketAcceptedNotifiedEvent>
	{

		private readonly ITicketAutoResponseService _ticketAutoResponseService;

		public TicketAutoResponseHandler(ITicketAutoResponseService ticketAutoResponseService)
		{
			_ticketAutoResponseService = ticketAutoResponseService;
		}

		public async Task Handle(TicketAcceptedNotifiedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.FirstAssignedDate == notification.AssignedDate) // fire auto-response logic only for the first ever assignment
			{
				var responseTypes = GetTicketAcceptedNotifiedResponseType(notification.Level, notification.EscalationReason);
				var userType = GetUserType(notification.AdvocateId, notification.IsSuperSolver);
				var model = new BrandResponseTemplateModel(notification.AdvocateFirstName);
				if (responseTypes != null)
				{
					await _ticketAutoResponseService.SendResponses(notification.BrandId, notification.TicketId, responseTypes, userType, model: model, authorId: notification.AdvocateId);
				}
			}
		}

		private UserType GetUserType(Guid? advocateId = null, bool? isSuperSolver = null) =>
			(advocateId, isSuperSolver) switch
			{
				(null, _) => UserType.SolvyBot,
				(_, true) => UserType.SuperSolver,
				(_, false) => UserType.Advocate,
				_ => UserType.Unknown
			};

		private BrandAdvocateResponseType[] GetTicketAcceptedNotifiedResponseType(TicketLevel level, TicketEscalationReason? escalationReason) =>
			(level, escalationReason) switch
			{
				(TicketLevel.Regular, _) => new[] { BrandAdvocateResponseType.TicketAcceptedLevel1 },
				(TicketLevel.SuperSolver, TicketEscalationReason.Tag) => new[] { BrandAdvocateResponseType.TicketAcceptedLevel2TagEscalated },
				(TicketLevel.SuperSolver, _) => new[] { BrandAdvocateResponseType.TicketAcceptedLevel2NonTagEscalated },
				(_, _) => null
			};
	}
}