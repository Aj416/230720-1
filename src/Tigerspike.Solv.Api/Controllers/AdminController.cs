using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Admin;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Enums;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Commands.Invoice;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.Scheduler;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// Invoice Controller
	/// </summary>
	[ApiVersion("1.0")]
	[Authorize(Roles = SolvRoles.Admin)]
	[Route("v{version:apiVersion}/admin")]
	public class AdminController : ApiController
	{
		private readonly IUserService _userService;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;
		private readonly ILogger<AdminController> _logger;
		private readonly IFeatureManager _featureManager;
		private readonly IRequestClient<StartInvoicingCycleCommand> _startInvoicingCycleClient;
		private readonly IInvoicingService _invoicingService;
		private readonly IInvoiceService _invoiceService;
		private readonly ISchedulerService _schedulerService;

		/// <summary>
		/// Invoice Constructor
		/// </summary>
		public AdminController(
			IFeatureManager featureManager,
			IRequestClient<StartInvoicingCycleCommand> startInvoicingCycleClient,
			IInvoicingService invoicingService,
			IInvoiceService invoiceService,
			ISchedulerService schedulerService,
			IUserService userService,
			IBus bus,
			IOptions<BusOptions> busOptionsAccessor,
			IDomainNotificationHandler notifications,
			IMediatorHandler mediator,
			ILogger<AdminController> logger) : base(notifications, mediator)
		{
			_featureManager = featureManager;
			_startInvoicingCycleClient = startInvoicingCycleClient;
			_invoicingService = invoicingService;
			_invoiceService = invoiceService;
			_schedulerService = schedulerService;
			_userService = userService;
			_bus = bus;
			_busOptions = busOptionsAccessor.Value;
			_logger = logger;
		}

		/// <summary>
		/// Schedules invoice generation
		/// </summary>
		[HttpGet("start-invoicing-cycle")]
		[FeatureGate(FeatureFlags.InvoicingCycleManualManagment)]
		[ProducesResponseType(typeof(StartInvoicingCycleCommand), StatusCodes.Status200OK)]
		public async Task<IActionResult> StartInvoicingCycle([FromQuery] DateTime? startDate)
		{
			if (await _featureManager.IsEnabledAsync(nameof(FeatureFlags.InvoicingMicroService)))
			{
				var result = await _invoicingService.StartInvoicingCycle(startDate);
				return result.Success ? Response() : Response(StatusCodes.Status400BadRequest);
			}

			var defaultStartDate = DateTime.UtcNow.Date.StartOfWeek(DayOfWeek.Monday).AddDays(-7);
			var inputDate = startDate ?? defaultStartDate;

			var response = await _startInvoicingCycleClient.GetResponse<StartInvoicingCycleResult>(
				new StartInvoicingCycleCommand(inputDate.Date));

			return response.Message.Success ? Response() : Response(StatusCodes.Status400BadRequest);
		}

		/// <summary>
		/// Schedules recurring invoicing cycles.
		/// </summary>
		[HttpGet("recurring-invoicing-cycle/{state}")]
		[FeatureGate(FeatureFlags.InvoicingCycleManualManagment)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> RecurringInvoicingCycle(bool state)
		{
			if (await _featureManager.IsEnabledAsync(nameof(FeatureFlags.InvoicingMicroService)))
			{
				await _invoicingService.RecurringInvoicingCycle(state);
				return Response(new { state });
			}
			else
			{
				await _schedulerService.SetRecurringJob(new RecurringInvoicingCycleCommand(_busOptions.Cron.InvoicingCycleWeeklySchedule), state);
				return Response(new { state });
			}
		}

		/// <summary>
		/// Execute a set of payments from brands to advocates (total sum of prices for solved tickets) based on a AdvocateInvoice
		/// This endpoint is not a business requirement, and it is only for internal use.
		/// This is for making manual payment during the initial pilot with the first client.
		/// once we are comfortable to go with automatic payment, this will become redundant (or not?).
		/// </summary>
		[HttpPost("pay/advocate/{advocateInvoiceId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> PayAdvocateInvoice(Guid advocateInvoiceId)
		{
			if (await _featureManager.IsEnabledAsync(nameof(FeatureFlags.InvoicingMicroService)))
			{
				await _invoicingService.PayAdvocateInvoice(advocateInvoiceId);
				return Response();
			}
			else
			{
				await _invoiceService.PayAdvocateInvoice(advocateInvoiceId);
				return Response();
			}
		}

		/// <summary>
		/// Execute a payment from brand to platform (fees + VAT) based on a ClientInvoice
		/// This endpoint is not a business requirement, and it is only for internal use.
		/// This is for making manual payment during the initial pilot with the first client.
		/// once we are comfortable to go with automatic payment, this will become redundant (or not?).
		/// </summary>
		[HttpPost("pay/platform/{clientInvoiceId}")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> PayClientInvoice(Guid clientInvoiceId)
		{
			if (await _featureManager.IsEnabledAsync(nameof(FeatureFlags.InvoicingMicroService)))
			{
				await _invoicingService.PayClientInvoice(clientInvoiceId);
				return Response();
			}
			else
			{
				await _invoiceService.PayClientInvoice(clientInvoiceId);
				return Response();
			}
		}

		/// <summary>
		/// Creates a new admin account.
		/// </summary>
		/// <param name="model">The admin create model.</param>
		/// <returns>201 if created.</returns>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminModel model)
		{
			await _userService.CreateAdmin(model.FirstName, model.LastName, model.Email, model.Password);

			return Response(StatusCodes.Status201Created);
		}

	}
}