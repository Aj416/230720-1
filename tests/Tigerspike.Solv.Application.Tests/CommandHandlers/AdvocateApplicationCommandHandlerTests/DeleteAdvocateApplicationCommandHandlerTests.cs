using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.AdvocateApplicationCommandHandlerTests
{
	public class DeleteAdvocateApplicationCommandHandlerTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenDeleting()
		{
			// Arrange
			var advocateApplication = Builder<AdvocateApplication>.CreateNew()
				.With(a => a.Id, Guid.NewGuid())
				.With(a => a.Email, "test@tigerspike.com")
				.With(a => a.DeletionHash, new string('0', 64))
				.Build();

			var cmd = new DeleteAdvocateApplicationCommand(advocateApplication.Email, advocateApplication.DeletionHash);

			MockAdvocateApplicationRepository.Setup(m => m.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<AdvocateApplication, bool>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IOrderedQueryable<AdvocateApplication>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IIncludableQueryable<AdvocateApplication, object>>>(),
					It.IsAny<bool>(), It.IsAny<bool>()))
				.ReturnsAsync(advocateApplication);

			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

			// Act
			var result = await AdvocateApplicationCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.True(result);

			MockAdvocateApplicationRepository.Verify(aar => aar.Delete(It.IsAny<AdvocateApplication>()),
				Times.Once);

			// There should be no domain notification (because it mainly means there is an error).
			MockMediator.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == cmd.MessageType)),
				Times.Never);
		}

		[Fact]
		public async Task ShouldFailWhenDeleting()
		{
			// Arrange
			var deleteHash = new string('0', 64);
			var advocateApplication = Builder<AdvocateApplication>
				.CreateNew()
				.With(a => a.Id, Guid.NewGuid())
				.With(a => a.Email, "test@tigerspike.com")
				.With(a => a.DeletionHash, new string('0', 64))
				.Build();

			var cmd = new DeleteAdvocateApplicationCommand(advocateApplication.Email, deleteHash);

			MockAdvocateApplicationRepository.Setup(m => m.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<AdvocateApplication, bool>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IOrderedQueryable<AdvocateApplication>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IIncludableQueryable<AdvocateApplication, object>>>(),
					It.IsAny<bool>(), It.IsAny<bool>()))
				.ReturnsAsync(advocateApplication);

			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

			// Act
			var result = await AdvocateApplicationCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.False(result);

			MockAdvocateApplicationRepository.Verify(aar => aar.Delete(It.IsAny<AdvocateApplication>()),
				Times.Once);

			MockMediator.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == cmd.MessageType)),
				Times.Once);
		}
	}
}