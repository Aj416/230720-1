using FluentValidation;

namespace Tigerspike.Solv.Application.Models
{
	public class BrandApplicationModelValidator : AbstractValidator<BrandApplicationModel>
	{
		public BrandApplicationModelValidator()
		{
			RuleFor(x => x.FirstName).NotEmpty();
			RuleFor(x => x.LastName).NotEmpty();
			RuleFor(x => x.CompanyName).NotEmpty();
			RuleFor(x => x.Email).NotEmpty().EmailAddress();
			RuleFor(x => x.Phone).NotEmpty();
		}
	}
}
