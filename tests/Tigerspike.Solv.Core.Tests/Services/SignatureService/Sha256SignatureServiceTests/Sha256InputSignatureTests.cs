using Tigerspike.Solv.Core.Services;
using Xunit;

namespace Tigerspike.Solv.Core.Tests.Services.SignatureService.HmacSha1SignatureServiceTests
{
	public class Sha256InputSignatureTests
	{

		[Fact]
		public void ShouldGenerateProperSignatureForBasicInput()
		{
			// Arrange
			var input = "this is basic input";
			var key = "secret";

			// Act
			var result = new Tigerspike.Solv.Core.Services.SignatureService().GenerateSha256(input, key);

			// Assert
			Assert.Equal("eeabb32d9f3bccfcca57597b06ce6296797f3b49f3463b8a1c3b1a2a3d2e7747", result); // as generated with https://www.freeformatter.com/hmac-generator.html#ad-output
		}

	}
}