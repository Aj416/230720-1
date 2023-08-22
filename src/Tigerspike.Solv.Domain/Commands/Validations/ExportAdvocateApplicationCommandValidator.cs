using FluentValidation;

namespace Tigerspike.Solv.Domain.Commands.Validations
{
	public class ExportAdvocateApplicationCommandValidator : AbstractValidator<ExportAdvocateApplicationCommand>
	{
		public ExportAdvocateApplicationCommandValidator()
		{
			RuleFor(c => c.Email)
				.NotEmpty()
				.Length(0, 254)
				.EmailAddress()
				.WithMessage("Email must be valid");

			RuleFor(x => x.ApplicationJson).NotEmpty().WithMessage("Application must not be blank");
		}
	}
}