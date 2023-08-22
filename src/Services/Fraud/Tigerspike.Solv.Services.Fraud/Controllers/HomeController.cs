using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Services.Fraud.Application.Commands.Ticket;
using Tigerspike.Solv.Services.Fraud.Application.Services;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Enum;
using Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces;
using Tigerspike.Solv.Services.Fraud.Models;

namespace Tigerspike.Solv.Services.Fraud.Controllers
{
	/// <summary>
	/// Invoice Controller
	/// </summary>
	[ApiVersion("1.0")]
	[Route("")]
	public class HomeController : ApiController
	{
		private readonly ISearchService<FraudSearchModel> _fraudSearchService;
		private readonly IFraudService _fraudService;

		/// <summary>
		/// HomeController constructor.
		/// </summary>
		public HomeController(
			ISearchService<FraudSearchModel> fraudSearchService,
			IFraudService fraudService,
			IDomainNotificationHandler notifications,
			IMediatorHandler mediator) : base(
			notifications, mediator)
		{
			_fraudSearchService = fraudSearchService;
			_fraudService = fraudService;
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpGet]
		public IActionResult Get() => Ok("Solv Fraud Service");

		/// <summary>
		/// Search fraud index using the specified criteria model
		/// </summary>
		[HttpGet("search")]
		[ProducesResponseType(typeof(IPagedList<FraudSearchModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> FraudSearch([FromQuery] FraudSearchCriteriaModel model) => Response(await _fraudSearchService.Search(model));

		/// <summary>
		/// Mark fraud status on the tickets
		/// </summary>
		[HttpPost("status/{fraudStatus}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> SetFraudStatus([FromRoute] FraudStatus fraudStatus, [FromBody] List<Guid> items)
		{
			await _mediator.SendCommand(new BulkSetTicketFraudStatusCommand(fraudStatus, items));

			return Response();
		}

		/// <summary>
		/// Get fraud ticket detail for a ticket id.
		/// </summary>
		[HttpGet("ticket/{ticketId}")]
		[ProducesResponseType(typeof(TicketModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetTicket([FromRoute] Guid ticketId) => Response(await Task.Run(() => _fraudService.GetTicketDetails(ticketId)));

		/// <summary>
		/// Get count of tickets with valid ticket detection.
		/// </summary>
		[HttpGet("fraud-count")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetFraudTicketCount() => Response(new { Count = await _fraudSearchService.GetCount(FraudStatus.FraudSuspected) });

		/// <summary>
		/// Builds the fraud ticket index. Used in non-production environemts
		/// </summary>
		[HttpPost("index-fraud")]
		[ProducesResponseType(typeof(RebuildResult), StatusCodes.Status200OK)]
		public async Task<IActionResult> BuildFraudIndex() =>
			Response(await _fraudSearchService.Rebuild());
	}
}