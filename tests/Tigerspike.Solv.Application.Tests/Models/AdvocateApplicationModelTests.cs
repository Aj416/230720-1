using FizzWare.NBuilder;
using Tigerspike.Solv.Application.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Models
{
	public class AdvocateApplicationModelTests
	{
		[Fact]
		public void ShouldSucceedWhenSanitiseModel()
		{
			//Arrange
			var model = Builder<AdvocateApplicationModel>.CreateNew()
				.With(x => x.FirstName = " john")
				.With(x => x.LastName = "       SMITH ")
				.Build();

			//Act
			model.Sanitize();

			//Assert
			Assert.StartsWith("J", model.FirstName);
			Assert.StartsWith("S", model.LastName);
		}
	}
}