using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Events;

namespace Tigerspike.Solv.Application.EventHandlers
{
	public class IdentityEventHandler :
		INotificationHandler<AdvocateIdentityCreatedEvent>,
		INotificationHandler<ClientIdentityCreatedEvent>,
		INotificationHandler<AdminIdentityCreatedEvent>,
		INotificationHandler<SuperSolverIdentityCreatedEvent>
	{
		private readonly IMediatorHandler _mediator;

		public IdentityEventHandler(IMediatorHandler mediator) => _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

		public async Task Handle(AdvocateIdentityCreatedEvent notification, CancellationToken cancellationToken) =>
			await _mediator.SendCommand(new CreateAdvocateCommand(notification.UserId, notification.FirstName, notification.LastName, notification.Email, notification.Phone, notification.CountryCode, notification.Source, notification.InternalAgent, verified: notification.InternalAgent, false));

		public async Task Handle(ClientIdentityCreatedEvent notification, CancellationToken cancellationToken) =>
			await _mediator.SendCommand(new CreateClientCommand(notification.UserId, notification.BrandId, notification.FirstName, notification.LastName, notification.Email, notification.Phone));
		public async Task Handle(SuperSolverIdentityCreatedEvent notification, CancellationToken cancellationToken) =>
				await _mediator.SendCommand(new CreateAdvocateCommand(notification.UserId, notification.FirstName, notification.LastName, notification.Email, notification.Phone, notification.CountryCode, source: null, internalAgent: false, verified: true, superSolver: true));

		public async Task Handle(AdminIdentityCreatedEvent notification, CancellationToken cancellationToken) =>
			await _mediator.SendCommand(new CreateAdminCommand(notification.UserId, notification.FirstName, notification.LastName, notification.Email));
	}
}
