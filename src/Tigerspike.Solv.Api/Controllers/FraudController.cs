using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tigerspike.Solv.Api.Services;
using Tigerspike.Solv.Application.Models.Fraud;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Enums;
using RebuildResult = Tigerspike.Solv.Application.Models.Fraud.RebuildResult;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// Advocate Application controller
	/// </summary>
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/fraud")]
	public class FraudController : ApiController
	{
		private readonly IFraudService _fraudService;

		/// <summary>
		/// FraudController constructor.
		/// </summary>
		public FraudController(
			IFraudService fraudService,
			IDomainNotificationHandler notifications,
			IMediatorHandler mediator) : base(
			notifications, mediator) => _fraudService = fraudService ?? throw new NullReferenceException(nameof(fraudService));

		/// <summary>
		/// Search fraud index using the specified criteria model
		/// </summary>
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpGet("search")]
		[ProducesResponseType(typeof(IPagedList<FraudSearchModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> FraudSearch([FromQuery] FraudSearchCriteriaModel model) => Response(await _fraudService.SearchAsync(model));

		/// <summary>
		/// Mark fraud status on the tickets
		/// </summary>
		[HttpPost("{fraudStatus}")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> SetFraudStatus([FromRoute] FraudStatus fraudStatus, [FromBody] List<Guid> items)
		{
			await _fraudService.SetStatus(fraudStatus, items);

			return Response(StatusCodes.Status204NoContent);
		}

		/// <summary>
		/// Get fraud ticket detail for particular ticket id.
		/// </summary>
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpGet("ticket/{ticketId}")]
		[ProducesResponseType(typeof(FraudTicketModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> FraudTicket([FromRoute] Guid ticketId) => Response(await _fraudService.GetTicketAsync(ticketId));

		/// <summary>
		/// Get fraud ticket count.
		/// </summary>
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpGet("count")]
		[ProducesResponseType(typeof(FraudCount), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetFraudCount() => Response(await _fraudService.GetFraudTicketCountAsync());

		/// <summary>
		/// Build Fraud Index.
		/// </summary>
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpPost("index")]
		[ProducesResponseType(typeof(RebuildResult), StatusCodes.Status200OK)]
		public async Task<IActionResult> BuildFraudIndex()
		{
			return Response(await _fraudService.BuildFraudIndex());
		}
	}
}