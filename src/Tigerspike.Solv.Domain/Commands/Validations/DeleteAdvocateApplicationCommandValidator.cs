using FluentValidation;

namespace Tigerspike.Solv.Domain.Commands.Validations
{
	public class DeleteAdvocateApplicationCommandValidator : AbstractValidator<DeleteAdvocateApplicationCommand>
	{
		public DeleteAdvocateApplicationCommandValidator()
		{
			RuleFor(c => c.Email)
				.NotEmpty()
				.Length(0, 254)
				.EmailAddress()
				.WithMessage("Email must be valid");

			RuleFor(c => c.Key)
				.NotEmpty()
				.WithMessage("Key is required")
				.Length(64, 64)
				.WithMessage("Invalid key")
				.Matches("^[a-f0-9]*$")
				.WithMessage("Invalid key");
		}
	}
}