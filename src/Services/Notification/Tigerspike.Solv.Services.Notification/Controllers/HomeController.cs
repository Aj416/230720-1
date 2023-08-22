using Microsoft.AspNetCore.Mvc;

namespace Tigerspike.Solv.Services.Notification.Controllers
{
	[Route("")]
	public class HomeController : ControllerBase
	{
		[HttpGet]
		public IActionResult Get() => Ok("Solv Notification Service");
	}
}