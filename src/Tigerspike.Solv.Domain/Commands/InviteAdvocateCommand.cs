using System;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Commands.Validations;

namespace Tigerspike.Solv.Domain.Commands
{
	public class InviteAdvocateCommand : Command<bool>
	{
		/// <summary>
		/// The application ID of the advocate.
		/// </summary>
		public Guid AdvocateApplicationId { get; set; }

		public InviteAdvocateCommand(Guid advocateApplicationId) => AdvocateApplicationId = advocateApplicationId;

		public override bool IsValid() => new InviteAdvocateCommandValidator().Validate(this).IsValid;
	}
}