using System.Threading.Tasks;
using MassTransit;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Messaging.IdentityVerification;

namespace Tigerspike.Solv.Application.Consumers.IdentityVerification
{
	public class IdentityCheckCreatedEventConsumer : IConsumer<IIdentityCheckCreatedEvent>
	{
		private readonly IMediatorHandler _mediator;

		public IdentityCheckCreatedEventConsumer(IMediatorHandler mediator) => _mediator = mediator;

		public Task Consume(ConsumeContext<IIdentityCheckCreatedEvent> context)
		{
			var notification = context.Message;
			return _mediator.SendCommand(
				new SetIdentityCheckDetailsCommand(notification.AdvocateId, notification.CheckId,
					notification.CheckReportUrl));
		}
	}
}