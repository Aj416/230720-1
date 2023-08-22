using System;
using System.Net;
using System.Security.Claims;
using System.Threading;
using AutoMoqCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Xunit;

namespace Tigerspike.Solv.Services.Fraud.Tests.Controllers
{
	public abstract class BaseController<T> where T : ApiController
	{
		protected readonly AutoMoqer Mocker = new AutoMoqer();

		private T _systemUnderTest;
		protected T SystemUnderTest
		{
			get
			{
				if (_systemUnderTest == null)
				{
					_systemUnderTest = Mocker.Resolve<T>();
				}

				return _systemUnderTest;
			}
		}

		public BaseController()
		{ }

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