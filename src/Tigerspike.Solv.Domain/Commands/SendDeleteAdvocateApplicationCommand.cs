using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Commands.Validations;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SendDeleteAdvocateApplicationCommand : Command<bool>
	{
		/// <summary>
		/// The applicants email
		/// </summary>
		public string Email { get; set; }

		public SendDeleteAdvocateApplicationCommand(string email)
		{
			Email = email;
		}

		public override bool IsValid()
		{
			ValidationResult = new SendDeleteAdvocateApplicationCommandValidator().Validate(this);
			return ValidationResult.IsValid;
		}
	}
}