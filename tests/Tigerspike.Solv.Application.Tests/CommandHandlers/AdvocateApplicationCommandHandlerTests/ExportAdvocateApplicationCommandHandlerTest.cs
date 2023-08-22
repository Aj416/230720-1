using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Messaging.Notification;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.AdvocateApplicationCommandHandlerTests
{
	public class ExportAdvocateApplicationCommandHandlerTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenExporting()
		{
			// Arrange
			var serviceSettings = MockServiceSettings.Object.Value;

			var advocateApplication = new AdvocateApplication("Sheesa", "Hore", "sheesa@email.com", "+123 456 789",
				"ca", "us", "Facebook", false, "102, Down Street", "Los Angeles", "10001");

			var cmd = new ExportAdvocateApplicationCommand(advocateApplication.Email, "{}");

			MockAdvocateApplicationRepository.Setup(m => m.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<AdvocateApplication, AdvocateApplication>>>(),
					It.IsAny<Expression<Func<AdvocateApplication, bool>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IOrderedQueryable<AdvocateApplication>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IIncludableQueryable<AdvocateApplication, object>
					>>(),
					It.IsAny<bool>(), It.IsAny<bool>()))
				.ReturnsAsync(advocateApplication);

			MockSendEndpoint.Setup(x =>
				x.Send(It.IsAny<ISendEmailMessageCommand>(),
					It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

			MockServiceSettings.Setup(ss => ss.Value).Returns(serviceSettings);

			MockAdvocateApplicationRepository.Setup(m => m.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<AdvocateApplication, AdvocateApplication>>>(),
					It.IsAny<Expression<Func<AdvocateApplication, bool>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IOrderedQueryable<AdvocateApplication>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IIncludableQueryable<AdvocateApplication, object>
					>>(),
					It.IsAny<bool>(), It.IsAny<bool>()))
				.ReturnsAsync(advocateApplication);

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
				.ReturnsAsync(new PagedList<AdvocateApplication>(new List<AdvocateApplication>(), 0, 25, 0));

			// Act
			var result = await AdvocateApplicationCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.True(result);

			MockSendEndpoint.Verify(x =>
				x.Send(It.IsAny<ISendEmailMessageCommand>(),
					It.IsAny<CancellationToken>()), Times.Once);

			// There should be no domain notification (because it mainly means there is an error).
			MockMediator.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == cmd.MessageType)),
				Times.Never);

		}
	}
}