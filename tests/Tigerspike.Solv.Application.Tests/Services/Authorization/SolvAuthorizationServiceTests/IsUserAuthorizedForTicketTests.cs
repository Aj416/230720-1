using System.Linq;
using System;
using System.Security.Claims;
using AutoMoqCore;
using Tigerspike.Solv.Application.Services;
using Xunit;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Constants;
using Moq;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Services.Authorization;

namespace Tigerspike.Solv.Application.Tests.Services.Authorization.SolvAuthorizationServiceTests
{
	public class IsUserAuthorizedForTicketTests
	{

		private readonly AutoMoqer Mocker = new AutoMoqer();

		[Fact]
		public async Task ShouldReturnFalseWhenAttemptToAuthorizeForNoTickets()
		{
			// Arrange
			var user = GetUser(SolvRoles.Advocate);

			// Act
			var systemUnderTest = Mocker.Resolve<SolvAuthorizationService>();
			var result = await systemUnderTest.IsAuthorizedToViewTicket(user);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task ShouldReturnFalseWhenUserIsInAdminRole()
		{
			// Arrange
			var ticketId = Guid.NewGuid();
			var user = GetUser(SolvRoles.Admin);

			// Act
			var systemUnderTest = Mocker.Resolve<SolvAuthorizationService>();
			var result = await systemUnderTest.IsAuthorizedToViewTicket(user, ticketId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task ShouldReturnFalseWhenUserIsInClientRole()
		{
			// Arrange
			var ticketId = Guid.NewGuid();
			var user = GetUser(SolvRoles.Client);

			// Act
			var systemUnderTest = Mocker.Resolve<SolvAuthorizationService>();
			var result = await systemUnderTest.IsAuthorizedToViewTicket(user, ticketId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task ShouldReturnFalseWhenAdvocateIsNotAuthorizedForSpecifiedTicket()
		{
			// Arrange
			var ticketId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = GetUser(SolvRoles.Advocate, userId);

			Mocker.GetMock<ITicketService>()
				.Setup(x => x.CanView(user, ticketId))
				.ReturnsAsync(false);

			// Act
			var systemUnderTest = Mocker.Resolve<SolvAuthorizationService>();
			var result = await systemUnderTest.IsAuthorizedToViewTicket(user, ticketId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task ShouldReturnFalseWhenCustomerHasDifferentTicketInToken()
		{
			// Arrange
			var ticketId = Guid.NewGuid();
			var wrongTicketId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = GetUser(SolvRoles.Customer, userId, wrongTicketId);

			Mocker.GetMock<ITicketService>()
				.Setup(x => x.CanView(user, ticketId))
				.ReturnsAsync(true);

			// Act
			var systemUnderTest = Mocker.Resolve<SolvAuthorizationService>();
			var result = await systemUnderTest.IsAuthorizedToViewTicket(user, ticketId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task ShouldThrowAnExceptionWhenAttemptToAuthorizeForMoreThanOneTokenForCustomer()
		{
			// Arrange
			var ticketIds = new [] { Guid.NewGuid(), Guid.NewGuid() };
			var userId = Guid.NewGuid();
			var user = GetUser(SolvRoles.Customer, userId, ticketIds.First());

			// Act
			var systemUnderTest = Mocker.Resolve<SolvAuthorizationService>();
			var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => systemUnderTest.IsAuthorizedToViewTicket(user, ticketIds));
		}

		[Fact]
		public async Task ShouldReturnTrueWhenCustomerIsAuthorized()
		{
			// Arrange
			var ticketId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = GetUser(SolvRoles.Customer, userId, ticketId);

			Mocker.GetMock<ITicketService>()
				.Setup(x => x.CanView(user, ticketId))
				.ReturnsAsync(true);

			// Act
			var systemUnderTest = Mocker.Resolve<SolvAuthorizationService>();
			var result = await systemUnderTest.IsAuthorizedToViewTicket(user, ticketId);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task ShouldReturnTrueWhenAdvocateIsAuthorized()
		{
			// Arrange
			var ticketId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = GetUser(SolvRoles.Advocate, userId);

			Mocker.GetMock<ITicketService>()
				.Setup(x => x.CanView(user, ticketId))
				.ReturnsAsync(true);

			// Act
			var systemUnderTest = Mocker.Resolve<SolvAuthorizationService>();
			var result = await systemUnderTest.IsAuthorizedToViewTicket(user, ticketId);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task ShouldReturnTrueWhenAdvocateIsAuthorizedForEveryTicket()
		{
			// Arrange
			var ticketIds = new [] { Guid.NewGuid(), Guid.NewGuid() };
			var userId = Guid.NewGuid();
			var user = GetUser(SolvRoles.Advocate, userId);

			Mocker.GetMock<ITicketService>()
				.Setup(x => x.CanView(user, ticketIds))
				.ReturnsAsync(true);

			// Act
			var systemUnderTest = Mocker.Resolve<SolvAuthorizationService>();
			var result = await systemUnderTest.IsAuthorizedToViewTicket(user, ticketIds);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task ShouldReturnFalseWhenAdvocateIsNotAuthorizedForEveryTicket()
		{
			// Arrange
			var ticketIds = new [] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
			var authorizedTickets = ticketIds.Take(2).ToArray();
			var userId = Guid.NewGuid();
			var user = GetUser(SolvRoles.Advocate, userId);

			Mocker.GetMock<ITicketService>()
				.Setup(x => x.CanView(user, authorizedTickets))
				.ReturnsAsync(true);

			// Act
			var systemUnderTest = Mocker.Resolve<SolvAuthorizationService>();
			var result = await systemUnderTest.IsAuthorizedToViewTicket(user, ticketIds);

			// Assert
			Assert.False(result);
		}

		private ClaimsPrincipal GetUser(string role, Guid? userId = null, Guid? ticketId = null)
		{
			var user = new Mock<ClaimsPrincipal>();
			user
				.Setup(x => x.FindFirst(ClaimTypes.Role))
				.Returns(new Claim(ClaimTypes.Role, role));
			user
				.Setup(x => x.IsInRole(role))
				.Returns(true);

			userId = userId ?? Guid.NewGuid();
			var id = role == SolvRoles.Customer ? userId.ToString() : "auth0|" + userId.ToString();
			user
				.SetupGet(x => x.Identity.Name)
				.Returns(id);

			if (role == SolvRoles.Customer)

			if (ticketId.HasValue)
			{
				user
					.Setup(x => x.FindFirst(ClaimTypes.Sid))
					.Returns(new Claim(ClaimTypes.Sid, ticketId.Value.ToString()));
			}

			return user.Object;
		}
	}
}