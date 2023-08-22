
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.CommandHandlers.Brand;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Application.Tests.CommandsHandlers;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandHandlers.BrandCommandHandlerTests
{
	public class AssignBrandsToAdvocateApplicationsTests : BaseCommandHandlerTests<BrandCommandHandler>
	{
		public AssignBrandsToAdvocateApplicationsTests()
		{

		}

		[Fact]
		public async Task ShouldSucceedWhenAssigningBrand()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var advocateAppIds = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };

			Brand brand = Builder<Brand>
				.CreateNew()
				.With(x => x.Id, brandId)
				.With(x => x.AdvocateApplications, new List<AdvocateApplicationBrand>())
				.With(x => x.IsPractice, false)
				.Build();

			var cmd = new AssignBrandsToAdvocateApplicationsCommand(new[] { brandId }, advocateAppIds);

			Mocker.GetMock<IAdvocateApplicationRepository>().Setup(s => s.AreApplicationsNotInvited(
				It.IsAny<Guid[]>())).ReturnsAsync(true);

			Mocker.GetMock<IBrandRepository>().Setup(m => m.AreAssignable(
				It.IsAny<Guid[]>())).ReturnsAsync(true);

			Mocker.GetMock<IUnitOfWork>().Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

			// Act
			await SystemUnderTest.Handle(cmd, CancellationToken.None);

			// Assert
			// That the number of assigned applications is correct
			var expectedCount = advocateAppIds.Length;
			Mocker.GetMock<IAdvocateApplicationBrandRepository>().Verify(x => x.InsertAsync(It.Is<IEnumerable<AdvocateApplicationBrand>>(list => list.Count() == expectedCount), default));

			MediatorMock.Verify(
				m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == CommandHandler.CommitErrorKey)),
				Times.Never);
		}

		[Fact]
		public async Task ShouldFailWhenAnyApplicationDoesntExist()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var advocateAppIds = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };

			var cmd = new AssignBrandsToAdvocateApplicationsCommand(new[] { brandId }, advocateAppIds);

			Mocker.GetMock<IAdvocateApplicationRepository>().Setup(s => s.AreApplicationsNotInvited(
				It.IsAny<Guid[]>())).ReturnsAsync(false);

			// Act
			await SystemUnderTest.Handle(cmd, CancellationToken.None);

			// Assert
			// That an error was raised
			MediatorMock.Verify(
				m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == CommandHandler.CommitErrorKey)),
				Times.Once);
		}


		[Fact]
		public async Task ShouldFailWhenBrandDoesntExist()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var advocateAppIds = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };

			var cmd = new AssignBrandsToAdvocateApplicationsCommand(new[] { brandId }, advocateAppIds);

			Mocker.GetMock<IAdvocateApplicationRepository>().Setup(s => s.AreApplicationsNotInvited(
				It.IsAny<Guid[]>())).ReturnsAsync(true);

			Mocker.GetMock<IBrandRepository>().Setup(m => m.GetSingleOrDefaultAsync(
					It.IsAny<Expression<Func<Brand, bool>>>(),
					It.IsAny<Func<IQueryable<Brand>, IOrderedQueryable<Brand>>>(),
					It.IsAny<Func<IQueryable<Brand>, IIncludableQueryable<Brand, object>>>(),
					It.IsAny<bool>()))
				.ReturnsAsync((Brand)null);

			// Act
			await SystemUnderTest.Handle(cmd, CancellationToken.None);

			// Assert
			// That an error was raised
			MediatorMock.Verify(
				m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == CommandHandler.CommitErrorKey)),
				Times.Once);
		}
	}
}
