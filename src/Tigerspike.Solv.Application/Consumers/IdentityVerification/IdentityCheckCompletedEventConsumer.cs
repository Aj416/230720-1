using System;
using System.Threading.Tasks;
using MassTransit;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Messaging.IdentityVerification;

namespace Tigerspike.Solv.Application.Consumers.IdentityVerification
{
	public class IdentityCheckCompletedEventConsumer : IConsumer<IIdentityCheckCompletedEvent>
	{
		private readonly IAdvocateRepository _advocateRepository;
		private readonly IMediatorHandler _mediator;

		public IdentityCheckCompletedEventConsumer(
			IAdvocateRepository advocateRepository, IMediatorHandler mediator)
		{
			_advocateRepository = advocateRepository;
			_mediator = mediator;
		}

		public async Task Consume(ConsumeContext<IIdentityCheckCompletedEvent> context)
		{
			var notification = context.Message;

			var advocateId = await _advocateRepository.GetSingleOrDefaultAsync(
				selector: x => (Guid?)x.Id,
				predicate: x => x.IdentityCheckId == notification.CheckId
			);

			if (advocateId != null)
			{
				var status = notification.Success.HasValue && notification.Success.Value ? IdentityVerificationStatus.Completed : IdentityVerificationStatus.Failed;
				await _mediator.SendCommand(new SetIdentityVerificationStatusCommand(advocateId.Value, status));
			}
		}
	}
}