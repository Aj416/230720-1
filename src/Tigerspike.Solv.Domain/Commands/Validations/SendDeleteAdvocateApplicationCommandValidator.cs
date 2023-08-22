using FluentValidation;

namespace Tigerspike.Solv.Domain.Commands.Validations
{
	public class SendDeleteAdvocateApplicationCommandValidator : AbstractValidator<SendDeleteAdvocateApplicationCommand>
	{
		public SendDeleteAdvocateApplicationCommandValidator()
		{
			RuleFor(c => c.Email)
				.NotEmpty()
				.Length(0, 254)
				.EmailAddress()
				.WithMessage("Email must be valid");
		}
	}
}