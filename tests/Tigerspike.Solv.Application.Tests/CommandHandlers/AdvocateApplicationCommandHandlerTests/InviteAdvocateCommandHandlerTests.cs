using System;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using MassTransit;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Messaging.Notification;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.AdvocateApplicationCommandHandlerTests
{
	public class InviteAdvocateCommandHandlerTests : BaseClass
	{
		/// <summary>
		/// Invite new applicant
		/// </summary>
		/// <returns>Succeed</returns>
		[Fact]
		public async Task ShouldSucceedWhenInvitingNewApplicant()
		{
			// Arrange
			var advocateApplication = Builder<AdvocateApplication>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.FirstName, "John")
				.With(x => x.LastName, "Doe")
				.With(x => x.Email, "email")
				.With(x => x.State, "state")
				.With(x => x.Phone, "+123 456 789")
				.With(x => x.Country, "country").With(x => x.Source, "facebook")
				.With(x => x.InvitationDate, null)
				.With(x => x.CompletedEmailSent, true) //Questionnaire answered
				.With(x => x.ApplicationStatus,
					AdvocateApplicationStatus.New) //New applicant
				.Build();

			var cmd = new InviteAdvocateCommand(advocateApplication.Id);

			MockAdvocateApplicationRepository.Setup(ur => ur.FindAsync(cmd.AdvocateApplicationId))
				.ReturnsAsync(advocateApplication);

			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

			MockSendEndpoint.Setup(x =>
					x.Send(It.IsAny<ISendEmailMessageCommand>(),
						It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			// Act
			await AdvocateApplicationCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.True(advocateApplication.InvitationEmailSent);
			Assert.NotNull(advocateApplication.InvitationDate);

			MockAdvocateApplicationRepository.Verify(
				m => m.Update(
					It.Is<AdvocateApplication>(u => u.Id == cmd.AdvocateApplicationId && u.InvitationEmailSent)),
				Times.Once);

			// There should be no domain notification (because it mainly means there is an error).
			MockMediator.Verify(
				m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == CommandHandler.CommitErrorKey)),
				Times.Never);
		}

		/// <summary>
		/// Re-inviting previously invited applicant
		/// </summary>
		/// <returns>Succeed</returns>
		[Fact]
		public async Task ShouldSucceedWhenInvitingPreviouslyInvitedAdvocate()
		{
			// Arrange
			var advocateApplication = Builder<AdvocateApplication>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.FirstName, "John")
				.With(x => x.LastName, "Doe")
				.With(x => x.Email, "email")
				.With(x => x.State, "state")
				.With(x => x.Phone, "+123 456 789")
				.With(x => x.Country, "country").With(x => x.Source, "facebook")
				.With(x => x.InvitationDate, null)
				.With(x => x.CompletedEmailSent, true) //Questionnaire answered
				.With(x => x.ApplicationStatus,
					AdvocateApplicationStatus.Invited) //Previously invited applicant
				.Build();

			var cmd = new InviteAdvocateCommand(advocateApplication.Id);

			MockAdvocateApplicationRepository.Setup(ur => ur.FindAsync(cmd.AdvocateApplicationId))
				.ReturnsAsync(advocateApplication);

			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

			MockSendEndpoint.Setup(x =>
					x.Send<ISendEmailMessageCommand>(It.IsAny<ISendEmailMessageCommand>(),
						It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			// Act
			await AdvocateApplicationCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.True(advocateApplication.InvitationEmailSent);
			Assert.NotNull(advocateApplication.InvitationDate);

			MockAdvocateApplicationRepository.Verify(
				m => m.Update(
					It.Is<AdvocateApplication>(u => u.Id == cmd.AdvocateApplicationId && u.InvitationEmailSent)),
				Times.Once);

			// There should be no domain notification (because it mainly means there is an error).
			MockMediator.Verify(
				m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == CommandHandler.CommitErrorKey)),
				Times.Never);
		}

		/// <summary>
		/// Should fail when trying to invite an applicant who is already an approved advocate
		/// </summary>
		/// <returns>Fail</returns>
		[Fact]
		public async Task ShouldFailWhenInvitingExistingAdvocate()
		{
			// Arrange
			var advocateApplication = Builder<AdvocateApplication>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.FirstName, "John")
				.With(x => x.LastName, "Doe")
				.With(x => x.Email, "email")
				.With(x => x.State, "state")
				.With(x => x.Phone, "+123 456 789")
				.With(x => x.Country, "country").With(x => x.Source, "facebook")
				.With(x => x.InvitationDate, null)
				.With(x => x.CompletedEmailSent, true) //Questionnaire answered
				.With(x => x.ApplicationStatus,
					AdvocateApplicationStatus.AccountCreated) //Previously approved applicant
				.Build();

			var cmd = new InviteAdvocateCommand(advocateApplication.Id);

			MockSendEndpoint.Setup(x =>
					x.Send(It.IsAny<ISendEmailMessageCommand>(),
						It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			// Act
			await AdvocateApplicationCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.False(advocateApplication.InvitationEmailSent);
			Assert.Null(advocateApplication.InvitationDate);

			MockAdvocateApplicationRepository.Verify(m =>
				m.Update(It.Is<AdvocateApplication>(u =>
					u.Id == cmd.AdvocateApplicationId && u.InvitationEmailSent)), Times.Never);

			MockMediator.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == cmd.MessageType)),
				Times.Once);
		}
	}
}