using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.Scheduler;
using Tigerspike.Solv.Messaging.Invoicing;
using Tigerspike.Solv.Services.Invoicing.Application.Commands;
using Tigerspike.Solv.Services.Invoicing.Application.Services;
using Tigerspike.Solv.Services.Invoicing.Enums;
using Tigerspike.Solv.Services.Invoicing.Models;

namespace Tigerspike.Solv.Services.Invoicing.Controllers
{
	/// <summary>
	/// Invoice Controller
	/// </summary>
	[ApiVersion("1.0")]
	[Route("")]
	public class HomeController : ApiController
	{
		private readonly BusOptions _busOptions;
		private readonly IRequestClient<IStartInvoicingCycleCommand> _startInvoicingCycleClient;
		private readonly ISchedulerService _schedulerService;
		private readonly IInvoiceService _invoiceService;

		public HomeController(
			IRequestClient<IStartInvoicingCycleCommand> startInvoicingCycleClient,
			ISchedulerService schedulerService,
			IInvoiceService invoiceService,
			IOptions<BusOptions> busOptionsAccessor,
			IDomainNotificationHandler notifications,
			IMediatorHandler mediator) : base(
			notifications, mediator)
		{
			_busOptions = busOptionsAccessor.Value;
			_startInvoicingCycleClient = startInvoicingCycleClient;
			_schedulerService = schedulerService;
			_invoiceService = invoiceService;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public IActionResult Get() => Ok("Solv Invoicing Service");

		/// <summary>
		/// Create Billing Details
		/// </summary>
		[HttpPost("{brandId}/billing-details")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateBillingDetails(Guid brandId,
			[FromBody] BillingDetailsModel billingDetailsModel)
		{
			if (billingDetailsModel != null)
			{
				await _invoiceService.CreateBillingDetails(brandId, billingDetailsModel);
			}
			else
			{
				NotifyError("Invoice Service", "Billing details cannot be null", StatusCodes.Status400BadRequest);
			}

			return Response();
		}

		/// <summary>
		/// Schedules invoice generation
		/// </summary>
		[HttpGet("start-invoicing-cycle")]
		[ProducesResponseType(typeof(IStartInvoicingCycleResult), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> StartInvoicingCycle([FromQuery] DateTime? startDate)
		{
			var defaultStartDate = DateTime.UtcNow.Date.StartOfWeek(DayOfWeek.Monday).AddDays(-7);
			var inputDate = startDate ?? defaultStartDate;

			var response =
				await _startInvoicingCycleClient.GetResponse<IStartInvoicingCycleResult>(
					new { StartDate = inputDate.Date });

			return response.Message.Success ? Response(response.Message) : Response(StatusCodes.Status400BadRequest);
		}

		/// <summary>
		/// Returns advocate invoices
		/// </summary>
		[HttpGet("advocates")]
		[ProducesResponseType(typeof(IPagedList<AdvocateInvoiceModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> GetCurrentAdvocateInvoices(
			[FromServices] IInvoiceService invoiceService,
			[FromQuery] PagedRequestModel pageRequest,
			[FromQuery] SortOrder sortOrder = SortOrder.Desc,
			[FromQuery] InvoiceSortBy sortBy = InvoiceSortBy.CreatedDate
		)
		{
			var invoicingCycleId = await invoiceService.GetLastInvoicingCycleId();
			return invoicingCycleId.HasValue ?
				Response(await invoiceService.GetAdvocateInvoiceList(invoicingCycleId.Value, pageRequest, sortOrder, sortBy)) :
				NoContent();
		}

		/// <summary>
		/// Returns list of payments of the advocate
		/// </summary>
		[HttpGet("{advocateId}/advocate-invoices")]
		[ProducesResponseType(typeof(IPagedList<AdvocateInvoiceModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetAdvocateInvoices(Guid advocateId) => Response(await _invoiceService.GetAdvocateInvoiceList(advocateId));

		/// <summary>
		/// Returns list of brand invoices (paged)
		/// </summary>
		[HttpGet("{brandId}/brand-invoices")]
		[ProducesResponseType(typeof(IPagedList<ClientInvoiceModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetBrandInvoices(Guid brandId, [FromQuery] PagedRequestModel pageRequest,
			[FromQuery] SortOrder sortOrder = SortOrder.Desc,
			[FromQuery] InvoiceSortBy sortBy = InvoiceSortBy.CreatedDate) => Response(await _invoiceService.GetClientInvoiceList(brandId, pageRequest, sortOrder, sortBy));

		/// <summary>
		/// Returns invoice data
		/// </summary>
		[HttpGet("{brandId}/invoices/{invoiceId}")]
		[ProducesResponseType(typeof(ClientInvoicePrintableModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetBrandInvoice(Guid brandId, Guid invoiceId) => Response(await _invoiceService.GetClientInvoicePrintable(invoiceId, brandId));

		/// <summary>
		/// Returns invoice data
		/// </summary>
		[HttpGet("advocate-invoices/{invoiceId}")]
		[ProducesResponseType(typeof(AdvocateInvoicePrintableModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetAdvocateInvoice(Guid invoiceId)
		{
			var model = await _invoiceService.GetAdvocateInvoicePrintable(invoiceId);
			return model != null ? Response(model) : NotFound();
		}

		/// <summary>
		/// Returns invoice data
		/// </summary>
		[HttpGet("{advocateId}/advocate-invoices/{invoiceId}")]
		[ProducesResponseType(typeof(AdvocateInvoicePrintableModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetAdvocateInvoice(Guid advocateId, Guid invoiceId)
		{
			var model = await _invoiceService.GetAdvocateInvoicePrintable(invoiceId, advocateId);
			return model != null ? Response(model) : NotFound();
		}

		/// <summary>
		/// Execute a payment from brand to platform (fees + VAT) based on a ClientInvoice
		/// This endpoint is not a business requirement, and it is only for internal use.
		/// This is for making manual payment during the initial pilot with the first client.
		/// once we are comfortable to go with automatic payment, this will become redundant (or not?).
		/// </summary>
		[HttpPost("pay/platform/{clientInvoiceId}")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> PayClientInvoice(Guid clientInvoiceId)
		{
			await _invoiceService.PayClientInvoice(clientInvoiceId);
			return Response();
		}

		/// <summary>
		/// Execute a set of payments from brands to advocates (total sum of prices for solved tickets) based on a AdvocateInvoice
		/// This endpoint is not a business requirement, and it is only for internal use.
		/// This is for making manual payment during the initial pilot with the first client.
		/// once we are comfortable to go with automatic payment, this will become redundant (or not?).
		/// </summary>
		[HttpPost("pay/advocate/{advocateInvoiceId}")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> PayAdvocateInvoice(Guid advocateInvoiceId)
		{
			await _invoiceService.PayAdvocateInvoice(advocateInvoiceId);
			return Response();
		}

		/// <summary>
		/// Schedules recurring invoicing cycles.
		/// </summary>
		[HttpGet("recurring-invoicing-cycle/{state}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> RecurringInvoicingCycle(bool state)
		{
			await _schedulerService.SetRecurringJob(
				new NewRecurringInvoicingCycleCommand(_busOptions.Cron.InvoicingCycleWeeklySchedule), state);

			return Response(new { State = state });
		}
	}
}
