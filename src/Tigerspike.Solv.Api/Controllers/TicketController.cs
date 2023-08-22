using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using ServiceStack;
using ServiceStack.Redis;
using Tigerspike.Solv.Api.Authentication.ApiKey;
using Tigerspike.Solv.Api.Authentication.Sdk;
using Tigerspike.Solv.Api.Extensions;
using Tigerspike.Solv.Api.Models;
using Tigerspike.Solv.Application.Enums;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Application.Models.Statistics;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Enums;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Models.Search;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Search.Interfaces;
using static Tigerspike.Solv.Core.Constants.CacheKeys;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// Ticket Controller
	/// </summary>
	[ApiVersion("1.0")]
	[Microsoft.AspNetCore.Mvc.Route("v{version:apiVersion}/tickets")]
	public class TicketController : ApiController
	{
		private const string ApiKeyAndSdkAuthenticationSchemes = ApiKeyAuthentication.Scheme + "," + SdkAuthentication.Scheme;

		/// <summary>
		/// Constant for user key
		/// </summary>
		public const string UserKey = "userId";

		/// <summary>
		/// Constant for brand key
		/// </summary>
		public const string Brandkey = "brandId";

		private readonly ITicketService _ticketService;
		private readonly ITicketUrlService _ticketUrlService;
		private readonly IBrandService _brandService;
		private readonly ISolvAuthorizationService _authorizationService;
		private readonly ISearchService<TicketSearchModel> _ticketSearchService;
		private readonly IFeatureManager _featureManager;
		private readonly IBus _bus;
		private readonly IUserService _userService;
		private readonly ITimestampService _timestampService;
		private readonly EmailTemplatesOptions _emailTemplateOptions;
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly StorageOptions _storageOptions;
		private readonly ILogger<TicketController> _logger;

		/// <summary>
		/// Ticket controller constructor
		/// </summary>
		public TicketController(
			ITicketService ticketService,
			ITicketUrlService ticketUrlService,
			IBrandService brandService,
			ISolvAuthorizationService authorizationService,
			ISearchService<TicketSearchModel> ticketSearchService,
			IDomainNotificationHandler notificationHandler,
			IFeatureManager featureManager,
			IBus bus,
			IUserService userService,
			IRedisClientsManager redisClientsManager, Microsoft.Extensions.Options.IOptions<EmailTemplatesOptions> emailTemplateOptions,
			ITimestampService timestampService, Microsoft.Extensions.Options.IOptions<StorageOptions> storageOptions,
			IMediatorHandler mediator,
			ILogger<TicketController> logger)
			: base(notificationHandler, mediator)
		{
			_authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
			_ticketSearchService = ticketSearchService ?? throw new ArgumentNullException(nameof(ticketSearchService));
			_featureManager = featureManager;
			_bus = bus;
			_userService = userService;
			_timestampService = timestampService;
			_ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
			_ticketUrlService = ticketUrlService;
			_brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
			_emailTemplateOptions = emailTemplateOptions?.Value ?? throw new ArgumentNullException(nameof(emailTemplateOptions));
			_redisClientsManager = redisClientsManager;
			_storageOptions = storageOptions.Value;
			_logger = logger;
		}

		/// <summary>
		/// Gets all assigned tickets.
		/// </summary>
		/// <returns>Tickets assigned to advocate.</returns>
		[HttpGet("advocate")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(List<TicketModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAdvocateCurrentTickets()
		{
			var tickets = await _ticketService.GetAdvocateTickets(User.GetId(), User.GetLevel());
			await _ticketService.TrimSensitiveInformation(await _userService.GetAccessLevel(User), tickets.ToArray());
			FillOnlineCustomers(tickets.ToArray());
			return Response(tickets);
		}

		/// <summary>
		/// Creates a ticket with the passed information.
		/// </summary>
		/// <returns>
		/// 201 if successful with location header has the url to the ticket for the customer
		/// </returns>
		[HttpPost]
		[Authorize(AuthenticationSchemes = ApiKeyAndSdkAuthenticationSchemes)]
		[ProducesResponseType(typeof(TicketCreatedModel), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CreateTicket([FromBody] CreateTicketModel createTicketModel)
		{
			createTicketModel.Sanitize();

			var ticketId = await _ticketService.Submit(createTicketModel, User.GetBrandId());

			if (ticketId != null)
			{
				var url = await _ticketUrlService.GetChatUrl(ticketId.Value, false);
				var token = await _ticketUrlService.GetCustomerToken(ticketId.Value);
				return CreatedAtAction(nameof(GetTicket), new { TicketId = ticketId }, new TicketCreatedModel(ticketId.Value, token, url));
			}

			return Response();
		}

		/// <summary>
		/// Gets the customer token for the specified ticket.
		/// </summary>
		/// <returns>
		/// 200 if successful
		/// </returns>
		[FeatureGate(FeatureFlags.GetCustomerToken)]
		[HttpGet("{ticketId}/customer-token")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetCustomerToken(Guid ticketId)
		{
			if (ticketId.Equals(Guid.Empty))
			{
				return BadRequest();
			}

			var token = await _ticketUrlService.GetCustomerToken(ticketId);

			return Response(new TokenModel { Token = token });
		}

		/// <summary>
		/// Reserve a ticket for the advocate.
		/// </summary>
		/// <returns>The new ticket if found</returns>
		[HttpPost("reserve")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> Reserve()
		{
			var ticketId = await _ticketService.Reserve(User.GetId(), User.IsInRole(SolvRoles.SuperSolver));
			return Response(ticketId.HasValue ? new { ticketId } : null);
		}

		/// <summary>
		/// Gets the ticket with corresponding ticketId.
		/// </summary>
		/// <param name="ticketId">The ticket id.</param>
		/// <returns>The ticket with corresponding ticketId; otherwise, null.</returns>
		[HttpGet("{ticketId}")]
		[Authorize]
		[ProducesResponseType(StatusCodes.Status410Gone)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(TicketModel), StatusCodes.Status200OK)]
		//[ProducesResponseType(typeof(CustomerTicketModel), StatusCodes.Status200OK)] // this can also be returned by this endpoint - might be good idea to separate those in future
		public async Task<IActionResult> GetTicket(Guid ticketId)
		{
			if (await _authorizationService.IsAuthorizedToViewTicket(User, ticketId))
			{
				if (User.IsInRole(SolvRoles.Customer))
				{
					return Response(await _ticketService.GetCustomerTicket(ticketId));
				}
				else
				{
					var ticket = await _ticketService.GetTicket(ticketId, await _userService.GetAccessLevel(User));
					await _ticketService.TrimSensitiveInformation(await _userService.GetAccessLevel(User), ticket);
					FillOnlineCustomers(ticket);
					return Response(ticket);
				}
			}

			return Forbid();
		}

		/// <summary>
		/// Sets specified tags for the specified ticket
		/// </summary>
		[HttpPost("{ticketId}/tags")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(typeof(IEnumerable<TagModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> SetTags(Guid ticketId, [FromBody] Guid[] tagIds)
		{
			if (await _authorizationService.IsAuthorizedToEditTicket(User, SolvOperationEnum.SetTags, ticketId))
			{
				await _ticketService.SetTags(ticketId, tagIds, User.GetLevel());
				return Response(await _ticketService.GetTags(ticketId, User.GetLevel()));
			}
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Accept the ticket that was reserved before, by the currently signed-in advocate.
		/// </summary>
		[HttpPost("{ticketId}/accept")]
		[Authorize(Roles = SolvRoles.Advocate)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> AcceptTicket(Guid ticketId)
		{
			await _ticketService.Accept(ticketId);
			return Response();
		}

		/// <summary>
		/// Reject the ticket that was reserved before, by the currently signed-in advocate.
		/// </summary>
		[HttpPost("{ticketId}/reject")]
		[Authorize(Roles = SolvRoles.Advocate)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> RejectTicket(Guid ticketId, [MinLength(1)][FromBody] int[] reasonIds)
		{
			if (await _authorizationService.IsAuthorizedToEditTicket(User, SolvOperationEnum.RejectTicket, ticketId))
			{
				await _ticketService.Reject(ticketId, reasonIds);
				return Response();
			}

			return Forbid();
		}

		/// <summary>
		/// Mark ticket as "resumed" from notification link
		/// </summary>
		[HttpPost("{ticketId}/resume")]
		[Authorize(Roles = SolvRoles.Customer)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> ResumeChatOnTicket(Guid ticketId)
		{
			if (await _authorizationService.IsAuthorizedToEditTicket(User, SolvOperationEnum.ResumeChat, ticketId))
			{
				await _ticketService.MarkResumption(ticketId);
				return Response();
			}

			return Forbid();
		}

		/// <summary>
		/// Gets all rejection reasons.
		/// </summary>
		[HttpGet("reject-reasons")]
		[Authorize(Roles = SolvRoles.Advocate)]
		[ProducesResponseType(typeof(List<RejectReasonModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetRejectReasons() => Response(await _ticketService.GetRejectReasons());

		/// <summary>
		/// Transition the ticket to next state, based upon the ticket state
		/// </summary>
		[HttpPost("{ticketId}/transition")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> TransitionTicket(Guid ticketId)
		{
			if (await _authorizationService.IsAuthorizedToEditTicket(User, SolvOperationEnum.TransitionTicket, ticketId))
			{
				var response = await _ticketService.Transition(ticketId);
				return Response(response);
			}

			return Forbid();
		}

		/// <summary>
		/// Sets specified spos details for the specified ticket
		/// </summary>
		[HttpPost("{ticketId}/spos")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> SetSpos(Guid ticketId, [FromBody] TicketSposModel model)
		{
			if (await _authorizationService.IsAuthorizedToEditTicket(User, SolvOperationEnum.SetSpos, ticketId))
			{
				await _ticketService.SetSpos(ticketId, model);
				return Response();
			}
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Sets specified spos details for the specified ticket
		/// </summary>
		[HttpPost("{ticketId}/diagnosis")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> SetDiagnosis(Guid ticketId, [FromBody] TicketDiagnosisModel model)
		{
			if (await _authorizationService.IsAuthorizedToEditTicket(User, SolvOperationEnum.SetDiagnosis, ticketId))
			{
				var success = await _ticketService.SetDiagnosis(ticketId, model);
				return success ? Response() : BadRequest();
			}
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Mark the ticket as abandoned by advocate
		/// </summary>
		/// <param name="ticketId">Id of the ticket</param>
		/// <param name="reasonIds">Array of reason ids</param>
		/// <returns></returns>
		[HttpPost("{ticketId}/abandon")]
		[Authorize(Roles = SolvRoles.Advocate)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> AbandonTicket(Guid ticketId, [MinLength(1)][FromBody] Guid[] reasonIds)
		{
			if (await _authorizationService.IsAuthorizedToEditTicket(User, SolvOperationEnum.AbandonTicket, ticketId))
			{
				await _ticketService.Abandon(ticketId, reasonIds);
				return Response();
			}

			return Forbid();
		}

		/// <summary>
		/// </summary>
		/// <param name="ticketId"></param>
		/// <returns></returns>
		[HttpPost("{ticketId}/reopen")]
		[Authorize(Roles = SolvRoles.Customer)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> ReopenTicket(Guid ticketId)
		{
			// let's check if his token allows him to see the ticket.
			if (!User.HasTokenForTicket(ticketId))
			{
				return Response(StatusCodes.Status403Forbidden);
			}

			await _ticketService.Reopen(ticketId);

			return Response();
		}

		/// <summary>
		/// Mark the ticket as closed by customer
		/// </summary>
		[HttpPost("{ticketId}/close")]
		[Authorize(Roles = SolvRoles.Customer)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CloseTicket(Guid ticketId, [FromBody] CloseTicketModel model)
		{
			// let's check if his token allows him to see the ticket.
			if (!User.HasTokenForTicket(ticketId))
			{
				return Response(StatusCodes.Status403Forbidden);
			}

			if (model.ClosedBy.HasValue)
			{
				await _ticketService.Close(ticketId, model.ClosedBy.Value);
			}
			else
			{
				await _ticketService.Close(ticketId);
			}

			return Response();
		}

		/// <summary>
		/// Mark the ticket as closed by customer
		/// </summary>
		[HttpPost("{ticketId}/complete")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CompleteTicket(Guid ticketId, [FromBody] CompleteTicketModel model)
		{
			// let's check if his token allows him to see the ticket.
			if (await _authorizationService.IsAuthorizedToEditTicket(User, SolvOperationEnum.CompleteTicket, ticketId))
			{
				await _ticketService.Complete(ticketId, model.TagStatus);
				return Response();
			}

			return Forbid();
		}

		/// <summary>
		/// Mark the ticket as escalated
		/// </summary>
		[HttpPost("{ticketId}/escalate")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> EscalateTicket(Guid ticketId)
		{
			await _ticketService.Escalate(ticketId);

			return Response();
		}

		/// <summary>
		/// Add the Ticket Complexity Rating
		/// </summary>
		[HttpPost("{ticketId}/complexity")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> SetComplexity(Guid ticketId, [FromBody] TicketComplexityModel model)
		{
			if (!ModelState.IsValid)
			{
				NotifyModelStateErrors();
				return Response(model);
			}

			if (await _authorizationService.IsAuthorizedToEditTicket(User, SolvOperationEnum.RateComplexity, ticketId))
			{
				await _ticketService.SetComplexity(ticketId, model.Complexity);
				return Response();
			}

			return Forbid();
		}

		/// <summary>
		/// Update the ticket csat value.
		/// </summary>
		[HttpPost("{ticketId}/csat")]
		[Authorize(Roles = SolvRoles.Customer)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(TicketModel), StatusCodes.Status204NoContent)]
		public async Task<IActionResult> SetCsat(Guid ticketId, [FromBody] TicketCsatModel model)
		{
			// let's check if his token allows him to see the ticket.
			if (!User.HasTokenForTicket(ticketId))
			{
				return Response(StatusCodes.Status403Forbidden);
			}

			await _ticketService.SetCsat(ticketId, model.Csat);

			return Response();
		}

		/// <summary>
		/// Update the ticket NPS value.
		/// </summary>
		[HttpPost("{ticketId}/nps")]
		[Authorize(Roles = SolvRoles.Customer)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(TicketModel), StatusCodes.Status204NoContent)]
		public async Task<IActionResult> SetNps(Guid ticketId, [FromBody] TicketNpsModel model)
		{
			// let's check if his token allows him to see the ticket.
			if (!User.HasTokenForTicket(ticketId))
			{
				return Response(StatusCodes.Status403Forbidden);
			}

			await _ticketService.SetNps(ticketId, model.Nps);

			return Response();
		}

		/// <summary>
		/// Search tickets index using the specified criteria model
		/// </summary>
		[Authorize]
		[HttpGet("search")]
		[ProducesResponseType(typeof(IPagedList<TicketSearchModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> TicketSearch([FromQuery] TicketSearchCriteriaModel model)
		{
			IPagedList<TicketSearchModel> page = null;

			if (User.IsInRole(SolvRoles.Advocate) || User.IsInRole(SolvRoles.SuperSolver))
			{
				model.AdvocateId = User.GetId();
				page = await _ticketSearchService.Search(model) as IPagedList<TicketSearchModel>;
			}
			else if (User.IsInRole(SolvRoles.Client))
			{
				model.BrandId = await _brandService.GetClientBrandId(User.GetId());
				page = await _ticketSearchService.Search(model) as IPagedList<TicketSearchModel>;
			}
			else if (User.IsInRole(SolvRoles.Admin))
			{
				page = await _ticketSearchService.Search(model) as IPagedList<TicketSearchModel>;
			}

			if (page?.Items != null)
			{
				await _ticketService.TrimSensitiveInformation(await _userService.GetAccessLevel(User), Enumerable.ToArray(page.Items));
			}

			return Response(page);
		}

		/// <summary>
		/// Gets tickets statistics by status.
		/// Role-based - returns data associated with role of user requesting data
		/// </summary>
		[Authorize]
		[HttpGet("statistics/status")]
		[ProducesResponseType(typeof(TicketStatisticByStatusModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetTicketStatisticsByStatus()
		{
			if (User.IsInRole(SolvRoles.Admin))
			{
				var result = await _ticketService.GetStatisticsByStatusForAll();
				return Response(result);
			}

			if (User.IsInRole(SolvRoles.Client))
			{
				var clientId = User.GetId();
				var brandId = await _brandService.GetClientBrandId(clientId);
				var result = await _ticketService.GetStatisticsByStatusForBrand(brandId);
				return Response(result);
			}

			if (User.IsInRole(SolvRoles.Advocate) || User.IsInRole(SolvRoles.SuperSolver))
			{
				var advocateId = User.GetId();
				var result = await _ticketService.GetStatisticsByStatusForAdvocate(advocateId);
				return Response(result);
			}

			return Forbid();
		}

		/// <summary>
		/// Gets tickets statistics overview.
		/// Role-based - returns data associated with role of user requesting data
		/// </summary>
		[Authorize]
		[HttpGet("statistics/overview")]
		[ProducesResponseType(typeof(TicketStatisticsOverviewModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetStatisticsOverview([FromQuery] TicketStatisticsRequestModel model)
		{
			if (User.IsInRole(SolvRoles.Admin))
			{
				var result = await _ticketService.GetStatisticsOverviewForAll(model.From, model.To);
				return Response(result);
			}

			if (User.IsInRole(SolvRoles.Client))
			{
				var clientId = User.GetId();
				var brandId = await _brandService.GetClientBrandId(clientId);
				var result = await _ticketService.GetStatisticsOverviewForBrand(brandId, model.From, model.To);
				return Response(result);
			}

			if (User.IsInRole(SolvRoles.Advocate) || User.IsInRole(SolvRoles.SuperSolver))
			{
				var advocateId = User.GetId();
				var result = await _ticketService.GetStatisticsOverviewForAdvocate(advocateId, model.From, model.To);
				return Response(result);
			}

			return Forbid();
		}

		/// <summary>
		/// Gets tickets statistics for escalated tickets. With an optional date range
		/// </summary>
		/// <param name="model">An optional date range</param>
		[Authorize(Roles = SolvRoles.Client)]
		[HttpGet("statistics/escalated")]
		[ProducesResponseType(typeof(TicketStatisticsForEscalatedModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetStatisticsForEscalated([FromQuery] TicketStatisticsRequestModel model)
		{
			var clientId = User.GetId();
			var brandId = await _brandService.GetClientBrandId(clientId);
			var result = await _ticketService.GetStatisticsForEscalated(brandId, model.From, model.To);
			return Response(result);
		}

		/// <summary>
		/// Gets billing cycle price statistic for client (brand)
		/// </summary>
		[Authorize(Roles = SolvRoles.Client)]
		[HttpGet("statistics/billing-cycle")]
		[ProducesResponseType(typeof(TicketStatisticsForBillingCycleModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetStatisticsForBillingCycle()
		{
			var clientId = User.GetId();
			var brandId = await _brandService.GetClientBrandId(clientId);

			var result = await _ticketService.GetStatisticsForCurrentPeriod(brandId);
			return Response(result);
		}

		/// <summary>
		/// Gets tickets performance overview for advocate for current week.
		/// </summary>
		/// <param name="brandId">The brand id that we want the data for (optional)</param>
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[HttpGet("statistics/performance/week")]
		[ProducesResponseType(typeof(TicketStatisticsPerformanceModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetPerformanceOverviewForCurrentWeek([FromQuery] Guid? brandId = null)
		{
			var from = _timestampService.GetUtcTimestamp().StartOfWeek();
			var result = await _ticketService.GetStatisticsPerformanceOverview(User.GetId(), brandId, from);
			return Response(result);
		}

		/// <summary>
		/// Gets tickets performance overview for advocate for all-time.
		/// </summary>
		/// <param name="brandId">The brand id that we want the data for (optional)</param>
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[HttpGet("statistics/performance/all-time")]
		[ProducesResponseType(typeof(TicketStatisticsPerformanceModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetPerformanceOverviewForAllTime([FromQuery] Guid? brandId = null) =>
			Response(await _ticketService.GetStatisticsPerformanceOverview(User.GetId(), brandId));

		/// <summary>
		/// Schedules ticket export
		/// </summary>
		/// <param name="model">Request parameters</param>
		[Authorize(Roles = SolvRoles.Admin + "," + SolvRoles.Client)]
		[HttpPost("export")]
		[ProducesResponseType(StatusCodes.Status202Accepted)]
		public async Task<IActionResult> CreateExport([FromQuery] TicketExportRequestModel model)
		{
			if (User.IsInRole(SolvRoles.Admin))
			{
				var userModel = await _userService.FindByUserId(User.GetId());
				await _bus.Send(new CreateTicketsExportCommand(userModel.Email, model.From.Value, model.To.Value, model.BrandId, CsvExportSource.Admin));
				return Accepted();
			}

			if (User.IsInRole(SolvRoles.Client))
			{
				var brand = await _brandService.GetClientBrand(User.GetId());
				if (brand.Id == model.BrandId && brand.TicketsExportEnabled)
				{
					var userModel = await _userService.FindByUserId(User.GetId());
					await _bus.Send(new CreateTicketsExportCommand(userModel.Email, model.From.Value, model.To.Value, model.BrandId, CsvExportSource.Client));
					return Accepted();
				}
			}

			return Forbid();
		}

		/// <summary>
		/// Return the number of available tickets for the passed brands
		/// </summary>
		[HttpGet("available")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetAvailableTicketsCount() => Response(new { Count = await _ticketService.GetAvailableTickets(User.GetId(), User.GetLevel()) });

		/// <summary>
		/// Upload tickets import to S3 bucket.
		/// </summary>
		[HttpPost("import")]
		[Authorize(Roles = SolvRoles.Admin + "," + SolvRoles.Client)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> UploadTicketsImport(TicketsImportModel importModel)
		{
			importModel.Metadata.TryGetValue(UserKey, out var userId);
			importModel.Metadata.TryGetValue(Brandkey, out var brandId);

			if (userId == null || brandId == null)
			{
				return BadRequest(new ProblemDetails() { Detail = "BrandId or UserId missing in request" });
			}

			if (User.IsInRole(SolvRoles.Client))
			{
				var id = await _brandService.GetClientBrandId(User.GetId());
				if (Guid.Parse(brandId.ToString()) != id)
				{
					return Forbid();
				}
			}

			if (Guid.Parse(userId.ToString()) != User.GetId())
			{
				return Forbid();
			}

			if (importModel.File.Length > _storageOptions.TicketsImportMaxSize)
			{
				return BadRequest(new ProblemDetails() { Detail = "Maximum file size exceeded" });
			}

			if (_storageOptions.TicketsImportAllowedExtensions.Contains(Path.GetExtension(importModel.File.FileName.ToLowerInvariant())) == false)
			{
				return BadRequest(new ProblemDetails() { Detail = "Provided file type is not supported" });
			}

			var timestamp = _timestampService.GetUtcTimestamp();
			var uploadPath = $"{_storageOptions.TicketsImportPrefix}/{brandId}/{timestamp.Date:yyyyMMdd}.csv";

			await using var fileStream = importModel.File.OpenReadStream();

			var key = await _brandService.UploadTicketsImportToS3Bucket(fileStream, uploadPath, _storageOptions.TicketsImportBucket, "text/csv", importModel.Metadata);
			return Response();
		}

		/// <summary>
		/// Get Import ticket list
		/// </summary>
		[Authorize(Roles = SolvRoles.Admin + "," + SolvRoles.Client)]
		[HttpGet("import")]
		[ProducesResponseType(typeof(IPagedList<TicketImportModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetTicketsImport(Guid brandId, int pageIndex = 0, int pageSize = 25,
		TicketImportSortBy sortBy = TicketImportSortBy.uploadDate, SortOrder sortOrder = SortOrder.Asc)
		{
			if (User.IsInRole(SolvRoles.Client))
			{
				var id = await _brandService.GetClientBrandId(User.GetId());
				if (Guid.Parse(brandId.ToString()) != id)
				{
					return Forbid();
				}
			}

			var importTickets = await _ticketService.GetImportTicket(brandId, pageIndex, pageSize, sortBy, sortOrder);

			return Response(StatusCodes.Status200OK, importTickets);
		}

		/// <summary>
		/// Exports failure ticket list on ticket import Id
		/// </summary>
		[Authorize(Roles = SolvRoles.Admin + "," + SolvRoles.Client)]
		[HttpGet("import/{ticketImportId}/failures")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> ImportFailureTicketListExportToCsv(Guid ticketImportid)
		{
			var advocateApplications = await _ticketService.GetAllFailureImportTicket(ticketImportid);
			return Content(advocateApplications, "text/csv");
		}

		/// <summary>
		/// Sets specified category for the specified ticket
		/// </summary>
		[HttpPost("{ticketId}/category/{categoryId}")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(typeof(CategoryModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> SetCategory(Guid ticketId, Guid categoryId)
		{
			if (await _authorizationService.IsAuthorizedToEditTicket(User, SolvOperationEnum.SetCategory, ticketId))
			{
				await _ticketService.SetCategory(ticketId, categoryId, User.GetId());
				return Response(await _ticketService.GetCategory(ticketId));
			}
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Sets valid transfer for the specified ticket.
		/// </summary>
		[HttpPost("{ticketId}/valid-transfer")]
		[Authorize(Roles = SolvRoles.SuperSolver)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> SetValidTransfer(Guid ticketId, [FromBody] TicketValidTransferModel model)
		{
			if (await _authorizationService.IsAuthorizedToEditTicket(User, SolvOperationEnum.SetValidTransfer, ticketId))
			{
				var success = await _ticketService.SetValidTransfer(ticketId, model);
				return success ? Response() : BadRequest();
			}
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Update the ticket feedback.
		/// </summary>
		[HttpPost("{ticketId}/additional-feedback")]
		[Authorize(Roles = SolvRoles.Customer)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> SetAdditionalFeedback(Guid ticketId, [FromBody] TicketAdditionalFeedBackModel model)
		{
			// let's check if his token allows him to see the ticket.
			if (!User.HasTokenForTicket(ticketId))
			{
				return Response(StatusCodes.Status403Forbidden);
			}

			await _ticketService.SetAdditionalFeedBack(ticketId, model.AdditionalFeedBack);

			return Response();
		}

		/// <summary>
		/// Fetch the customer online status from the cache and fill it for the ticket.
		/// </summary>
		private void FillOnlineCustomers(params TicketModel[] tickets)
		{
			if (!tickets.Any())
			{
				return;
			}
			using (var client = _redisClientsManager.GetClient())
			{
				// Fetch all customer online values for the tickets in one shot;
				var whoIsOnline = client.GetAll<long>(tickets.Select(ticket => GetCustomerConnectionsKey($"{ticket.Id}")));
				foreach (var ticket in tickets)
				{
					ticket.IsCustomerOnline = whoIsOnline[GetCustomerConnectionsKey($"{ticket.Id}")] > 0;
				}
			}
		}
	}
}