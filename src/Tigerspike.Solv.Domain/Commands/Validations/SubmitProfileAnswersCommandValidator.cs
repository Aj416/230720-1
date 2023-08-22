using FluentValidation;

namespace Tigerspike.Solv.Domain.Commands.Validations
{
	public class SubmitProfileAnswersCommandValidator : AbstractValidator<SubmitProfileAnswersCommand>
	{
		public SubmitProfileAnswersCommandValidator()
		{
			RuleFor(c => c.AdvocateApplicationId).NotNull().NotEmpty();

			RuleFor(c => c.ApplicationAnswers).NotNull();
		}
	}
}