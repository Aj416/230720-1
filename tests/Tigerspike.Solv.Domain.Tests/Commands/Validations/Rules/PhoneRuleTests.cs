using FluentValidation;
using Tigerspike.Solv.Domain.Commands.Validations;
using Xunit;

namespace Tigerspike.Solv.Domain.Tests.Commands.Validations.Rules
{
	public class PhoneRuleTests
	{
		private class TestObject
		{
			public string Value { get; set; }
		}
		private class TestValidator : AbstractValidator<TestObject>
		{
			public TestValidator()
			{
				RuleFor(x => x.Value).Phone();
			}
		}

		[Fact]
		public void ShouldSuccedWhenPhoneNumberIsValid()
		{
			// Arrange
			var obj = new TestObject { Value = "123456789" };

			// Act
			var result = new TestValidator().Validate(obj);

			// Assert
			Assert.True(result.IsValid);
		}

		[Fact]
		public void ShouldFailWhenPhoneNumberHasLetters()
		{
			// Arrange
			var obj = new TestObject { Value = "12345sss6789" };

			// Act
			var result = new TestValidator().Validate(obj);

			// Assert
			Assert.False(result.IsValid);
		}
	}
}