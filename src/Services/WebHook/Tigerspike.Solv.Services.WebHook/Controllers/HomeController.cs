using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tigerspike.Solv.Services.WebHook.Controllers
{
	[Route("")]
	public class HomeController : ControllerBase
	{
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public IActionResult Get() => Ok("Solv WebHook Service");

	}
}