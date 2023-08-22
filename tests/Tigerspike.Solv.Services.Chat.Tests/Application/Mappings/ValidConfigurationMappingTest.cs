using Xunit;

namespace Tigerspike.Solv.Chat.Tests.Application.Mappings
{
	public class ValidConfigurationMappingTests : BaseMappingTest
	{
		[Fact]
		public void ConfigurationOfAutoMapperShouldBeValid()
		{
			SystemUnderTest.ConfigurationProvider.AssertConfigurationIsValid();
		}
	}
}