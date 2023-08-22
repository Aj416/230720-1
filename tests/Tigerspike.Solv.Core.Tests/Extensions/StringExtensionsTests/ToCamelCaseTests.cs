using System;
using System.Collections.Generic;
using Tigerspike.Solv.Core.Extensions;
using Xunit;

namespace Tigerspike.Solv.Core.Tests.Extensions.StringExtensionsTests
{
    public class ToCamelCaseTests
    {

		[Fact]
		public void NullStringShouldBeConvertedToNull()
		{
			// Act
			var result = StringExtensions.ToCamelCase(null);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void EmptyStringShouldBeConvertedToEmptyString()
		{
			// Act
			var result = StringExtensions.ToCamelCase(string.Empty);

			// Assert
			Assert.Equal(string.Empty, result);
		}

		[Fact]
		public void SingleCharacterStringShouldBeConvertedSuccesfully()
		{
			// Act
			var result = StringExtensions.ToCamelCase("X");

			// Assert
			Assert.Equal("x", result);
		}

		[Fact]
		public void StringWithoutSpacesShouldBeConvertedSuccesfully()
		{
			// Act
			var result = StringExtensions.ToCamelCase("ThisIsTest");

			// Assert
			Assert.Equal("thisIsTest", result);
		}

    }
}