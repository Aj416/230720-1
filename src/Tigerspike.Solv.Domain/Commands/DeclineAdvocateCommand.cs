using System;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Commands.Validations;

namespace Tigerspike.Solv.Domain.Commands
{
	public class DeclineAdvocateCommand : Command<bool>
	{
		public DeclineAdvocateCommand(Guid advocateApplicationId) => AdvocateApplicationId = advocateApplicationId;

		/// <summary>
		/// The application ID of the advocate.
		/// </summary>
		public Guid AdvocateApplicationId { get; set; }

		public override bool IsValid() => new DeclineAdvocateCommandValidator().Validate(this).IsValid;
	}
}