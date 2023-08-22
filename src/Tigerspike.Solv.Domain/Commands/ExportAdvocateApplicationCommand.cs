using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Commands.Validations;

namespace Tigerspike.Solv.Domain.Commands
{
	public class ExportAdvocateApplicationCommand : Command<bool>
	{
		/// <summary>
		/// The applicants email
		/// </summary>
		public string Email { get; set; }

		public string ApplicationJson { get; }

		public ExportAdvocateApplicationCommand(string email, string applicationJson)
		{
			Email = email;
			ApplicationJson = applicationJson;
		}

		public override bool IsValid()
		{
			ValidationResult = new ExportAdvocateApplicationCommandValidator().Validate(this);
			return ValidationResult.IsValid;
		}
	}
}