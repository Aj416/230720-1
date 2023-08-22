using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// The home controller
	/// </summary>
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}")]
	public class HomeController : ApiController
	{

		private readonly IJwtService _jwtService;


		/// <summary>
		/// Home Controller
		/// </summary>
		public HomeController(
			IJwtService jwtService,
			IDomainNotificationHandler notifications,
			IMediatorHandler mediator) : base(notifications, mediator)
		{
			_jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
		}


		/// <summary>
		/// The main home controller get method.
		/// </summary>
		/// <returns></returns>
		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpGet]
		public IActionResult Get() => Ok("Solv API");


		/// <summary>
		/// Schedules invoice generation
		/// </summary>
		[HttpPost("sdk-token")]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		public IActionResult GetSdkToken(string applicationId)
		{
			if (string.IsNullOrEmpty(applicationId))
			{
				return Unauthorized();
			}

			return Response(new {Token = _jwtService.CreateSdkToken(applicationId).Token});
		}
	}
}