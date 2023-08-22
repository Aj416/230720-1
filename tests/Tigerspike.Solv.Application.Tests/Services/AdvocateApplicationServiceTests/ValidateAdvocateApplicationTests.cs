using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateApplicationServiceTests
{
	public class ValidateAdvocateApplicationTests : BaseClass
	{
		[Fact]
		public async Task ShouldReturnTrueWhenApplicationIsValid()
		{
			//Arrange
			MockAdvocateApplicationRepository
				.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<AdvocateApplication, bool>>>()))
				.ReturnsAsync(true);

			//Act
			var result = await AdvocateApplicationService.Validate(It.IsAny<Guid>());

			//Assert
			Assert.True(result);
		}

		[Fact]
		public async void ShouldReturnFalseWhenCompletedEmailIsSent()
		{
			//Arrange
			var application = GetAdvocateApplication(AdvocateApplicationStatus.New, true);

			MockAdvocateApplicationRepository
				.Setup(x => x.FindAsync(application.Id))
				.ReturnsAsync(application);

			//Act
			var result = await AdvocateApplicationService.Validate(application.Id);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public async void ShouldReturnFalseWhenApplicationIsInAccountCreatedStatus()
		{
			//Arrange
			var application = GetAdvocateApplication(AdvocateApplicationStatus.AccountCreated, false);

			MockAdvocateApplicationRepository
				.Setup(x => x.FindAsync(application.Id))
				.ReturnsAsync(application);

			//Act
			var result = await AdvocateApplicationService.Validate(application.Id);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public async void ShouldReturnFalseWhenApplicationIsInInvitedStatus()
		{
			//Arrange
			var application = GetAdvocateApplication(AdvocateApplicationStatus.Invited, false);

			MockAdvocateApplicationRepository
				.Setup(x => x.FindAsync(application.Id))
				.ReturnsAsync(application);

			//Act
			var result = await AdvocateApplicationService.Validate(application.Id);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public async void ShouldReturnFalseWhenApplicationIsInNotSuitableStatus()
		{
			//Arrange
			var application = GetAdvocateApplication(AdvocateApplicationStatus.NotSuitable, false);

			MockAdvocateApplicationRepository
				.Setup(x => x.FindAsync(application.Id))
				.ReturnsAsync(application);

			//Act
			var result = await AdvocateApplicationService.Validate(application.Id);

			//Assert
			Assert.False(result);
		}


		[Fact]
		public async void ShouldReturnFalseWhenApplicationIsNotFound()
		{
			//Act
			var result = await AdvocateApplicationService.Validate(Guid.Empty);

			//Assert
			Assert.False(result);
		}

		private AdvocateApplication GetAdvocateApplication(AdvocateApplicationStatus status, bool completedEmailSent)
		{
			return Builder<AdvocateApplication>
				.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.ApplicationStatus, status)
				.With(x => x.CompletedEmailSent, completedEmailSent)
				.Build();
		}
	}
}