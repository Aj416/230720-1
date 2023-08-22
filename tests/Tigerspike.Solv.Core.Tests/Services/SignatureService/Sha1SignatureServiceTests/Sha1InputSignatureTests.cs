using Tigerspike.Solv.Core.Services;
using Xunit;

namespace Tigerspike.Solv.Core.Tests.Services.SignatureService.HmacSha1SignatureServiceTests
{
	public class Sha1InputSignatureTests
	{

		[Fact]
		public void ShouldGenerateProperSignatureForBasicInput()
		{
			// Arrange
			var input = "this is basic input";
			var key = "secret";

			// Act
			var result = new Tigerspike.Solv.Core.Services.SignatureService().GenerateSha1(input, key);

			// Assert
			Assert.Equal("8a8150c3e86e2f562eb6d339649777b6367bfbaf", result); // as generated with https://www.freeformatter.com/hmac-generator.html#ad-output
		}

	}
}