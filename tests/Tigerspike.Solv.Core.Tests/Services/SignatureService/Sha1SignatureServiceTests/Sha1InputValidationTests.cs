using System;
using Xunit;

namespace Tigerspike.Solv.Core.Tests.Services.SignatureService.HmacSha1SignatureServiceTests
{
	public class Sha1InputValidationTests
	{

		[Fact]
		public void ShouldThrowArgumentNullExceptionWhenPayloadIsNull()
		{
			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new Tigerspike.Solv.Core.Services.SignatureService().GenerateSha1(null, Guid.NewGuid().ToString()));
}

		[Fact]
		public void ShouldThrowArgumentNullExceptionWhenKeyIsNull()
		{
			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new Tigerspike.Solv.Core.Services.SignatureService().GenerateSha1(Guid.NewGuid().ToString(), null));
		}

		[Fact]
		public void ShouldNotThrowExceptionWhenInputIsEmptyString()
		{
			// Act & Assert
			new Tigerspike.Solv.Core.Services.SignatureService().GenerateSha1(string.Empty, Guid.NewGuid().ToString());
		}

		[Fact]
		public void ShouldNotThrowExceptionWhenKeyIsEmptyString()
		{
			// Act & Assert
			new Tigerspike.Solv.Core.Services.SignatureService().GenerateSha1(Guid.NewGuid().ToString(), string.Empty);
		}

	}
}