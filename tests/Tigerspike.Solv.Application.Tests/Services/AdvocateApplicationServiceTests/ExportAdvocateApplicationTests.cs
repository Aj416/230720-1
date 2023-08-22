using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Admin;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Models.Profile;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateApplicationServiceTests
{
	public class ExportAdvocateApplicationTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenExportingAdvocateApplication()
		{
			//Arrange
			var advocateApplication = new AdvocateApplication("Sheesa", "Hore", "sheesa@email.com", "+123 456 789", "ca", "us", "Facebook", false, "102, Down Street", "Los Angeles", "10001");
			var advocateApplicationModel = new AdvocateApplicationModel();
			var adminAdvocateApplicationModel = new AdminAdvocateApplicationModel { Application = new AdvocateApplicationModel() };

			MockAdvocateApplicationRepository.Setup(m => m.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<AdvocateApplication, bool>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IOrderedQueryable<AdvocateApplication>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IIncludableQueryable<AdvocateApplication, object>>>(),
					It.IsAny<bool>(), It.IsAny<bool>()))
				.ReturnsAsync(advocateApplication);

			MockAdvocateApplicationRepository.Setup(m => m.GetFullAdvocateApplication(
					It.IsAny<Expression<Func<AdvocateApplication, bool>>>()
				))
				.ReturnsAsync(new[] { advocateApplication }.ToList());

			MockAreaRepository.Setup(m => m.GetPagedListAsync(
					It.IsAny<Expression<Func<Area, bool>>>(),
					It.IsAny<Func<IQueryable<Area>, IOrderedQueryable<Area>>>(),
					It.IsAny<Func<IQueryable<Area>, IIncludableQueryable<Area, object>>>(),
					It.IsAny<int>(),
					It.IsAny<int>(),
					It.IsAny<bool>(),
					It.IsAny<CancellationToken>(), It.IsAny<bool>()))
				.ReturnsAsync(new PagedList<Area>(new List<Area>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));

			MockUserRepository.Setup(m => m.FindAsync(It.IsAny<Guid>(), default(CancellationToken))).ReturnsAsync((User)null);

			MockAdvocateApplicationService.Setup(m => m.GetAdvocateApplication(It.IsAny<Guid>()))
				.ReturnsAsync(adminAdvocateApplicationModel);

			Mapper.Setup(x => x.Map<AdvocateApplicationModel>(It.IsAny<AdvocateApplication>()))
				.Returns(advocateApplicationModel);

			Mediator.Setup(m => m.SendCommand(It.IsAny<ExportAdvocateApplicationCommand>())).ReturnsAsync(true);

			//Act
			var result = await AdvocateApplicationService.ExportAdvocateApplication(advocateApplication.Email);

			//Assert
			Assert.True(result);
			Mediator.Verify(db => db.SendCommand(It.IsAny<ExportAdvocateApplicationCommand>()), Times.Exactly(1));
		}

		[Fact]
		public async void ShouldReturnFalseWhenExportingAdvocateApplication()
		{
			//Arrange
			var advocateApplication = new AdvocateApplication("Sheesa", "Hore", "sheesa@email.com", "+123 456 789", "ca", "us", "Facebook", false, "102, Down Street", "Los Angeles", "10001");
			var advocateApplicationModel = new AdvocateApplicationModel();
			var adminAdvocateApplicationModel = new AdminAdvocateApplicationModel { Application = new AdvocateApplicationModel() };

			MockAdvocateApplicationRepository.Setup(m => m.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<AdvocateApplication, bool>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IOrderedQueryable<AdvocateApplication>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IIncludableQueryable<AdvocateApplication, object>>>(),
					It.IsAny<bool>(), It.IsAny<bool>()))
				.ReturnsAsync(advocateApplication);

			MockAdvocateApplicationRepository.Setup(m => m.GetFullAdvocateApplication(
					It.IsAny<Expression<Func<AdvocateApplication, bool>>>()
				))
				.ReturnsAsync(new [] { advocateApplication }.ToList());

			MockAreaRepository.Setup(m => m.GetPagedListAsync(
					It.IsAny<Expression<Func<Area, bool>>>(),
					It.IsAny<Func<IQueryable<Area>, IOrderedQueryable<Area>>>(),
					It.IsAny<Func<IQueryable<Area>, IIncludableQueryable<Area, object>>>(),
					It.IsAny<int>(),
					It.IsAny<int>(),
					It.IsAny<bool>(),
					It.IsAny<CancellationToken>(), It.IsAny<bool>()))
				.ReturnsAsync(new PagedList<Area>(new List<Area>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));

			MockAdvocateApplicationService.Setup(m => m.GetAdvocateApplication(It.IsAny<Guid>()))
				.ReturnsAsync(adminAdvocateApplicationModel);

			MockUserRepository.Setup(m => m.FindAsync(It.IsAny<Guid>(), default(CancellationToken))).ReturnsAsync((User)null);

			Mapper.Setup(x => x.Map<AdvocateApplicationModel>(It.IsAny<AdvocateApplication>()))
				.Returns(advocateApplicationModel);

			Mediator.Setup(m => m.SendCommand(It.IsAny<ExportAdvocateApplicationCommand>())).ReturnsAsync(false);

			//Act
			var result = await AdvocateApplicationService.ExportAdvocateApplication("test@tigerspike.com");

			//Assert
			Assert.False(result);
			Mediator.Verify(db => db.SendCommand(It.IsAny<ExportAdvocateApplicationCommand>()), Times.Exactly(1));
		}
	}
}