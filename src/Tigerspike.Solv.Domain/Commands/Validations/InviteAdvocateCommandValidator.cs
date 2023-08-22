using FluentValidation;

namespace Tigerspike.Solv.Domain.Commands.Validations
{
	public class InviteAdvocateCommandValidator : AbstractValidator<InviteAdvocateCommand>
	{
		public InviteAdvocateCommandValidator()
		{
			RuleFor(c => c.AdvocateApplicationId)
			.NotEmpty()
			.WithMessage("Invalid AdvocateApplicationId");
		}
	}
}