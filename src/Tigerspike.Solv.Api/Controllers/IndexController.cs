using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Enums;
using Tigerspike.Solv.Search.Interfaces;
using Tigerspike.Solv.Search.Models;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// Advocate Controller
	/// </summary>
	[ApiVersion("1.0")]
	[Authorize(Roles = SolvRoles.Admin)]
	[FeatureGate(FeatureFlags.IndexBuild)]
	[Route("v{version:apiVersion}/index")]
	public class IndexController : Controller
	{
		private readonly ISearchService<TicketSearchModel> _ticketSearchService;
		private readonly ISearchService<AdvocateSearchModel> _advocateSearchService;
		private readonly ISearchService<AdvocateApplicationSearchModel> _advocateApplicationSearchService;

		/// <summary>
		/// A constructor ! You know! to construct objects!
		/// </summary>
		public IndexController(
			ISearchService<TicketSearchModel> ticketSearchService,
			ISearchService<AdvocateSearchModel> advocateSearchService,
			ISearchService<AdvocateApplicationSearchModel> advocateApplicationSearchService
			)
		{
			_ticketSearchService = ticketSearchService;
			_advocateSearchService = advocateSearchService;
			_advocateApplicationSearchService = advocateApplicationSearchService;
		}

		/// <summary>
		/// Rebuilds the ticket index
		/// </summary>
		[HttpPost("tickets")]
		[ProducesResponseType(typeof(RebuildResult), StatusCodes.Status200OK)]
		public async Task<IActionResult> RebuildTicketIndex()
		{
			return Ok(await _ticketSearchService.Rebuild());
		}

		/// <summary>
		/// Rebuilds the advocate index
		/// </summary>
		[HttpPost("advocates")]
		[ProducesResponseType(typeof(RebuildResult), StatusCodes.Status200OK)]
		public async Task<IActionResult> RebuildAdvocateIndex()
		{
			return Ok(await _advocateSearchService.Rebuild());
		}

		/// <summary>
		/// Rebuilds the advocate application index
		/// </summary>
		[HttpPost("advocateApplications")]
		[ProducesResponseType(typeof(RebuildResult), StatusCodes.Status200OK)]
		public async Task<IActionResult> RebuildAdvocateApplicationIndex()
		{
			return Ok(await _advocateApplicationSearchService.Rebuild());
		}
	}
}
