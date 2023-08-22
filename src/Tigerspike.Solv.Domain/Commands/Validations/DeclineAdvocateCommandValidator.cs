using FluentValidation;

namespace Tigerspike.Solv.Domain.Commands.Validations
{
	public class DeclineAdvocateCommandValidator : AbstractValidator<DeclineAdvocateCommand>
	{
		public DeclineAdvocateCommandValidator()
		{
			RuleFor(c => c.AdvocateApplicationId)
			.NotEmpty()
			.WithMessage("Invalid AdvocateApplicationId");
		}
	}
}