using AutoMoqCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Xunit;

namespace Tigerspike.Solv.Api.Tests.Controllers
{
	public abstract class BaseController<T> where T : ApiController
	{
		protected readonly AutoMoqer Mocker = new AutoMoqer();
		private readonly IDomainNotificationHandler _notificationHandler = new DomainNotificationHandler();
		private Mock<IMediatorHandler> MediatorMock => Mocker.GetMock<IMediatorHandler>();
		protected Mock<ClaimsPrincipal> PrincipalMock => Mocker.GetMock<ClaimsPrincipal>();
		protected readonly Guid UserId = Guid.NewGuid();

		private T _systemUnderTest;
		protected T SystemUnderTest
		{
			get
			{
				if (_systemUnderTest == null)
				{
					_systemUnderTest = Mocker.Resolve<T>();
					SystemUnderTest.ControllerContext = GetControllerContext();
				}

				return _systemUnderTest;
			}
		}

		public BaseController()
		{
			Mocker.SetInstance(_notificationHandler);
			MediatorMock
				.Setup(x => x.RaiseEvent(It.IsAny<DomainNotification>()))
				.Callback<DomainNotification>(x => _notificationHandler.Handle(x, CancellationToken.None));

			PrincipalMock
				.SetupGet(x => x.Identity.Name)
				.Returns($"auth0|{UserId}");

			Mocker.GetMock<IOptions<EmailTemplatesOptions>>()
				.Setup(x => x.Value)
				.Returns(new EmailTemplatesOptions());
		}

		private ControllerContext GetControllerContext()
		{
			return new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = PrincipalMock.Object
				}
			};
		}

		protected void Assert_StatusCode(int statusCode, IActionResult actionResult)
		{
			switch (actionResult)
			{
				case ObjectResult objectResult:
					Assert.Equal(statusCode, objectResult.StatusCode);
					break;

				case StatusCodeResult statusCodeResult:
					Assert.Equal(statusCode, statusCodeResult.StatusCode);
					break;

				case ForbidResult forbidResult:
					Assert.Equal(statusCode, (int)HttpStatusCode.Forbidden);
					break;

				default:
					Assert.True(false, "Action result does not contain any StatusCode");
					break;
			}
		}

	}
}