using System.Linq;
using System.Threading.Tasks;
using MockQueryable.Moq;
using Moq;
using Tigerspike.Solv.Core.Extensions;
using Xunit;

namespace Tigerspike.Solv.Core.Tests.Extensions.DateTimeExtensionsTests
{
	public class ToPagedListAsyncTest
	{

		[Fact]
		public async Task QueryShouldOnlyBeEvaluatedOnce()
		{
			// Arrange
			var queryMock = Enumerable.Empty<object>().AsQueryable().BuildMock();

			// Act
			var result = await QueryableExtensions.ToPagedListAsync(queryMock.Object, 0, 10);

			// Assert
			queryMock.Verify(x => x.Provider, Times.Exactly(2)); // one for Count, one for Items fetching
			queryMock.Verify(x => x.Expression, Times.Exactly(2)); // one for Count, one for Items fetching
			queryMock.Verify(x => x.GetEnumerator(), Times.Never); // explicit enumeration should never happen
		}

	}
}