using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Validators;

namespace Tigerspike.Solv.Domain.Commands.Validations
{
	public class PhoneRule : PropertyValidator
	{
		public PhoneRule() : base() { }
		protected override string GetDefaultMessageTemplate() => "Phone number is invalid";

		protected override bool IsValid(PropertyValidatorContext context)
		{
			var value = context.PropertyValue as string;
			return new PhoneAttribute().IsValid(value); // take the same validation as in request model

			// var regex = new Regex("^[0-9]+$"); // if we decide to stick with the 'just digits' validation
			// return regex.IsMatch(value);
		}
	}

	public static class PhoneRuleExtension
	{
		public static IRuleBuilderOptions<T, string> Phone<T>(this IRuleBuilder<T, string> ruleBuilder)
		{
			return ruleBuilder.SetValidator(new PhoneRule());
		}
	}
}