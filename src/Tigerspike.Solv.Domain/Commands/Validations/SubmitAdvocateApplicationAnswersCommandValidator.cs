using FluentValidation;

namespace Tigerspike.Solv.Domain.Commands.Validations
{
	public class SubmitAdvocateApplicationAnswersCommandValidator : AbstractValidator<SubmitAdvocateApplicationAnswersCommand>
	{
		public SubmitAdvocateApplicationAnswersCommandValidator()
		{
			RuleFor(c => c.AdvocateApplicationId).NotNull().NotEmpty();

			RuleFor(c => c.ApplicationAnswers).NotNull();
		}
	}
}