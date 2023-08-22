using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Profile;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateApplicationServiceTests
{
	public class AdminAdvocateApplicationsTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedGetAdminAdvocateApplications()
		{
			var applicationAnswerModelList = Builder<ApplicationAnswerModel>.CreateListOfSize(1).All().Build().ToList();

			var advocateApplicationModelList =
				Builder<AdvocateApplicationModel>.CreateListOfSize(1).All()
					.With(x => x.ApplicationAnswers = applicationAnswerModelList).Build().ToList();
			var advocateApplicationModelPagedList = new PagedList<AdvocateApplicationModel>(advocateApplicationModelList, It.IsAny<int>(),
				It.IsAny<int>(), It.IsAny<int>());

			var advocateApplicationList = Builder<AdvocateApplication>.CreateListOfSize(1).All().Build().ToList();
			var advocateApplicationPagedList = new PagedList<AdvocateApplication>(advocateApplicationList, It.IsAny<int>(), It.IsAny<int>(),
				It.IsAny<int>());

			//Arrange
			MockAdvocateApplicationRepository.Setup(m => m.GetPagedListAsync(
					It.IsAny<Expression<Func<AdvocateApplication, AdvocateApplication>>>(),
					It.IsAny<Expression<Func<AdvocateApplication, bool>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IOrderedQueryable<AdvocateApplication>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IIncludableQueryable<AdvocateApplication, object>
					>>(),
					It.IsAny<int>(),
					It.IsAny<int>(),
					It.IsAny<bool>(),
					It.IsAny<CancellationToken>(), It.IsAny<bool>()))
				.ReturnsAsync(advocateApplicationPagedList);

			Mapper.Setup(x => x.Map<PagedList<AdvocateApplicationModel>>(It.IsAny<PagedList<AdvocateApplication>>()))
				.Returns(advocateApplicationModelPagedList);

			//Act
			var result = await AdvocateApplicationService.GetAdminAdvocateApplications(AdvocateApplicationStatus.New,
				0, 25,
				AdminAdvocateApplicationStatusSortBy.CreatedDate, SortOrder.Asc);

			//Assert
			Assert.NotNull(result);
		}
	}
}