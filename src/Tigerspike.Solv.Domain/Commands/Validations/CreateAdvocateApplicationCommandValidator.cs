using FluentValidation;

namespace Tigerspike.Solv.Domain.Commands.Validations
{
	public class CreateAdvocateApplicationCommandValidator : AbstractValidator<CreateAdvocateApplicationCommand>
	{
		public CreateAdvocateApplicationCommandValidator()
		{
			RuleFor(c => c.Email)
				.NotEmpty()
				.Length(0, 254)
				.EmailAddress()
				.WithMessage("Email must be valid");

			RuleFor(c => c.IsAdult)
				.NotNull()
				.Equal("on")
				.WithMessage("Must be of legal age");

			RuleFor(c => c.MarketingCheckbox)
				.NotNull()
				.Equal("on")
				.WithMessage("Must consent to marketing communications");

			RuleFor(c => c.DataPolicyCheckbox)
				.NotNull()
				.Equal("on")
				.WithMessage("Must consent to Data Policy");

			RuleFor(c => c.FirstName)
				.NotEmpty()
				.Length(0, 200)
				.WithMessage("First name field must not be empty");

			RuleFor(c => c.LastName)
				.NotEmpty()
				.Length(0, 200)
				.WithMessage("Last name field must not be empty");

			RuleFor(c => c.Phone)
				.NotEmpty()
				.Length(0, 30)
				.Phone()
				.When(x => x.InternalAgent == false)
				.WithMessage("Phone number field must not be empty");

			RuleFor(c => c.Source)
				.NotEmpty()
				.Length(0, 50)
				.WithMessage("Source field must not be empty");

			RuleFor(c => c.Country)
				.NotEmpty()
				.Length(2, 2)
				.When(x => x.InternalAgent == false)
				.WithMessage("Country field must not be empty and have a length of two characters");

			RuleFor(c => c.State)
				.Length(0, 2)
				.WithMessage("State field must have a minimum length of 0 and a maximum length of 2 characters")
				.When(aa => aa.Country == "US");
		}
	}
}