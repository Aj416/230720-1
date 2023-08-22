using Xunit;

namespace Tigerspike.Solv.Application.Tests.Mappings
{
	public class ValidConfigurationMappingTests : BaseMappingTests
	{
		[Fact]
		public void ConfigurationOfAutoMapperShouldBeValid()
		{
			SystemUnderTest.ConfigurationProvider.AssertConfigurationIsValid();
		}
	}
}