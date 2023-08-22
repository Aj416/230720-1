using System;
using System.Collections.Generic;
using Tigerspike.Solv.Core.Extensions;
using Xunit;

namespace Tigerspike.Solv.Core.Tests.Extensions.DateTimeExtensionsTests
{
    public class StartOfWeekTest
    {

        public static IEnumerable<object[]> StartOfWeekTheoryData() 
        {
            yield return new object[] { new DateTime(2019, 12, 29), DayOfWeek.Monday, new DateTime(2019, 12, 23) };
            yield return new object[] { new DateTime(2019, 12, 30), DayOfWeek.Monday, new DateTime(2019, 12, 30) };
            yield return new object[] { new DateTime(2019, 12, 31), DayOfWeek.Monday, new DateTime(2019, 12, 30) };
            yield return new object[] { new DateTime(2020, 1, 1), DayOfWeek.Monday, new DateTime(2019, 12, 30) };
            yield return new object[] { new DateTime(2020, 1, 2), DayOfWeek.Monday, new DateTime(2019, 12, 30) };
            yield return new object[] { new DateTime(2020, 1, 3), DayOfWeek.Monday, new DateTime(2019, 12, 30) };
            yield return new object[] { new DateTime(2020, 1, 4), DayOfWeek.Monday, new DateTime(2019, 12, 30) };
            yield return new object[] { new DateTime(2020, 1, 5), DayOfWeek.Monday, new DateTime(2019, 12, 30) };
            yield return new object[] { new DateTime(2020, 1, 6), DayOfWeek.Monday, new DateTime(2020, 1, 6) };

            yield return new object[] { new DateTime(2019, 12, 28), DayOfWeek.Sunday, new DateTime(2019, 12, 22) };            
            yield return new object[] { new DateTime(2019, 12, 29), DayOfWeek.Sunday, new DateTime(2019, 12, 29) };
            yield return new object[] { new DateTime(2019, 12, 30), DayOfWeek.Sunday, new DateTime(2019, 12, 29) };
            yield return new object[] { new DateTime(2019, 12, 31), DayOfWeek.Sunday, new DateTime(2019, 12, 29) };
            yield return new object[] { new DateTime(2020, 1, 1), DayOfWeek.Sunday, new DateTime(2019, 12, 29) };
            yield return new object[] { new DateTime(2020, 1, 2), DayOfWeek.Sunday, new DateTime(2019, 12, 29) };
            yield return new object[] { new DateTime(2020, 1, 3), DayOfWeek.Sunday, new DateTime(2019, 12, 29) };
            yield return new object[] { new DateTime(2020, 1, 4), DayOfWeek.Sunday, new DateTime(2019, 12, 29) };
            yield return new object[] { new DateTime(2020, 1, 5), DayOfWeek.Sunday, new DateTime(2020, 1, 5) };
            yield return new object[] { new DateTime(2020, 1, 6), DayOfWeek.Sunday, new DateTime(2020, 1, 5) };

            yield return new object[] { new DateTime(2019, 12, 28, 5, 5, 5), DayOfWeek.Sunday, new DateTime(2019, 12, 22) };
            yield return new object[] { new DateTime(2020, 1, 4, 5, 5, 5), DayOfWeek.Sunday, new DateTime(2019, 12, 29) };
        }
        
        [Theory, MemberData(nameof(StartOfWeekTheoryData))]
        public void ShouldReturnProperStartOfWeekDate(DateTime input, DayOfWeek startOfWeek, DateTime expected)
        {
            // Act
            var result = DateTimeExtensions.StartOfWeek(input, startOfWeek);

            // Assert
            Assert.Equal(expected, result);
        }

    }
}