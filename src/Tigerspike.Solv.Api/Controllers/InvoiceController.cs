using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Enums;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// Controller
	/// </summary>
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/invoices")]
	public class InvoiceController : ApiController
	{
		private readonly IFeatureManager _featureManager;
		private readonly IInvoicingService _invoicingService;
		private readonly ILogger<InvoiceController> _logger;

		/// <summary>
		/// Constructor
		/// </summary>
		public InvoiceController(
			IFeatureManager featureManager,
			IInvoicingService invoicingService,
			IDomainNotificationHandler notifications,
			IMediatorHandler mediator,
			ILogger<InvoiceController> logger) : base(notifications, mediator)
		{
			_featureManager = featureManager;
			_invoicingService = invoicingService;
			_logger = logger;
		}

		/// <summary>
		/// Returns advocate invoices
		/// </summary>
		[Authorize(Roles = SolvRoles.Admin)]
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
			if (await _featureManager.IsEnabledAsync(nameof(FeatureFlags.InvoicingMicroService)))
			{
				return Response(await _invoicingService.GetCurrentAdvocateInvoices(pageRequest, sortOrder, sortBy));
			}

			var invoicingCycleId = await invoiceService.GetLastInvoicingCycleId();

			return invoicingCycleId.HasValue ?
				Response(await invoiceService.GetAdvocateInvoiceList(invoicingCycleId.Value, pageRequest, sortOrder, sortBy)) : NoContent();
		}
	}
}
