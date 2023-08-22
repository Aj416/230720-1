using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class UpdateAdvocatePaymentAccountCommand : Command
	{
		public Guid AdvocateId { get; }

		public UpdateAdvocatePaymentAccountCommand(Guid advocateId) => AdvocateId = advocateId;

		public override bool IsValid() => AdvocateId != Guid.Empty;
	}
}