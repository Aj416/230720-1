using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Commands.Validations;

namespace Tigerspike.Solv.Domain.Commands
{
	public class DeleteAdvocateApplicationCommand : Command<bool>
	{
		/// <summary>
		/// Email address of the advocate applicant
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// The secret key used to verify ownership of the application
		/// </summary>
		public string Key { get; set; }

		public DeleteAdvocateApplicationCommand(string email, string key)
		{
			Email = email;
			Key = key;
		}

		public override bool IsValid()
		{
			ValidationResult = new DeleteAdvocateApplicationCommandValidator().Validate(this);
			return ValidationResult.IsValid;
		}
	}
}