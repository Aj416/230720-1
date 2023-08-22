using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Infra.Bus.Configuration;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// Maintenance Controller
	/// </summary>
	[ApiVersion("1.0")]
	[Authorize(Roles = SolvRoles.Admin)]
	[Route("v{version:apiVersion}/maintenance")]
	public class MaintenanceController : ApiController
	{
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;
		private readonly IMaintenanceService _maintenanceService;

		/// <summary>
		/// Invoice Constructor
		/// </summary>
		public MaintenanceController(
			IMaintenanceService maintenanceService,
			IBus bus,
			IOptions<BusOptions> busOptionsAccessor,
			IDomainNotificationHandler notifications,
			IMediatorHandler mediator
		) : base(notifications, mediator)
		{
			_maintenanceService = maintenanceService;
			_bus = bus;
			_busOptions = busOptionsAccessor.Value;
		}
		/// <summary>
		/// Marks all users in Auth0 as verified
		/// </summary>
		[HttpPost("mark-all-emails-as-verified")]
		[ProducesResponseType(StatusCodes.Status202Accepted)]
		public IActionResult MarkAllEmailsAsVerified()
		{
			var task = _maintenanceService.MarkAllEmailsAsVerified(); // start this task in the background
			return Accepted();
		}

	}
}