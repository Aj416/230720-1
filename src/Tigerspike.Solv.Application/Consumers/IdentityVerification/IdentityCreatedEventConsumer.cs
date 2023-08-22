using System.Threading.Tasks;
using MassTransit;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Messaging.IdentityVerification;

namespace Tigerspike.Solv.Application.Consumers.IdentityVerification
{
	public class IdentityCreatedEventConsumer : IConsumer<IIdentityCreatedEvent>
	{
		private readonly IMediatorHandler _mediator;


		public IdentityCreatedEventConsumer(IMediatorHandler mediator) => _mediator = mediator;

		public Task Consume(ConsumeContext<IIdentityCreatedEvent> context)
		{
			var notification = context.Message;

			return _mediator.SendCommand(new SetIdentityApplicantCommand(notification.AdvocateId,
				notification.ApplicantId));
		}
	}
}