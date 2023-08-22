using FluentValidation;
using Tigerspike.Solv.Domain.Commands.Brand;

namespace Tigerspike.Solv.Domain.Commands.Validations
{
	public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
	{
		public CreateBrandCommandValidator()
		{
			RuleFor(c => c.Name)
				.NotEmpty()
				.MaximumLength(100);

			RuleFor(c => c.Logo)
				.NotEmpty()
				.MaximumLength(256);

			RuleFor(c => c.Thumbnail)
				.NotEmpty()
				.MaximumLength(256);

			RuleFor(c => c.Color)
				.MaximumLength(7)
				.Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
		}
	}
}