using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Refit;
using Tigerspike.Solv.Api.Models;
using Tigerspike.Solv.Api.Models.Advocate;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Advocate;
using Tigerspike.Solv.Application.Models.Induction;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Application.Models.Statistics;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Enums;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Infra.Data.Models;
using Tigerspike.Solv.Search.Interfaces;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// Advocate Controller
	/// </summary>
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/advocates")]
	public class AdvocateController : ApiController
	{
		private readonly IFeatureManager _featureManager;
		private readonly IAdvocateService _advocateService;
		private readonly IBrandService _brandService;
		private readonly IInvoicingService _invoicingService;
		private readonly IInvoiceService _invoiceService;
		private readonly IPaymentService _paymentService;
		private readonly IQuizService _quizService;
		private readonly ITicketService _ticketService;
		private readonly ISearchService<AdvocateSearchModel> _advocateSearchService;
		private readonly ILogger<AdvocateController> _logger;

		/// <summary>
		/// Advocate Constructor
		/// </summary>
		public AdvocateController(
			IFeatureManager featureManager,
			IAdvocateService advocateService,
			IBrandService brandService,
			IInvoicingService invoicingService,
			IInvoiceService invoiceService,
			IPaymentService paymentService,
			IQuizService quizService,
			ITicketService ticketService,
			ISearchService<AdvocateSearchModel> advocateSearchService,
			IDomainNotificationHandler notifications,
			IMediatorHandler mediator,
			ILogger<AdvocateController> logger) : base(notifications, mediator)
		{
			_featureManager = featureManager;
			_advocateService = advocateService ??
				throw new ArgumentNullException(nameof(advocateService));
			_brandService = brandService ??
				throw new ArgumentNullException(nameof(brandService));
			_invoicingService = invoicingService;
			_invoiceService = invoiceService;
			_advocateSearchService = advocateSearchService ??
				throw new ArgumentNullException(nameof(advocateSearchService));
			_paymentService = paymentService ??
				throw new ArgumentNullException(nameof(paymentService));
			_quizService = quizService;
			_ticketService = ticketService;
			_logger = logger;
		}

		/// <summary>
		/// Get advocate's information with the specified id
		/// </summary>
		/// <returns>Advocate info</returns>
		[HttpGet("{id}")]
		[Authorize(Roles = SolvRoles.Admin + "," + SolvRoles.Client)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(AdvocateModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAdvocate(Guid id)
		{
			Guid? brandId = null;

			if (User.IsInRole(SolvRoles.Client))
			{
				// Client can see advocates that belong to his brand only.
				brandId = await _brandService.GetClientBrandId(User.GetId());
			}

			var advocate = await _advocateService.FindAsync(id, brandId);
			if (advocate == null)
			{
				NotifyError("Advocate", "Advocate doesn't exist", (int)HttpStatusCode.NotFound);
			}

			return Response(advocate);
		}

		/// <summary>
		/// Creates a new advocate account from the invitation.
		/// </summary>
		/// <param name="model">The advocate create model.</param>
		/// <returns>201 if created.</returns>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateAdvocate([FromBody] AdvocateCreateModel model)
		{
			await _advocateService.Create(model.Token, model.Password);

			return Response(StatusCodes.Status201Created);
		}

		/// <summary>
		/// Check the application token if it is valid
		/// </summary>
		/// <returns>OK Response with no content if the token is valid</returns>
		[HttpPost("applications/validate-token")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> ValidateToken([FromBody] TokenModel model)
		{
			var result = await _advocateService.ValidateToken(model.Token);
			return result ? Response(StatusCodes.Status204NoContent) : Response(StatusCodes.Status400BadRequest);
		}

		/// <summary>
		/// Set the video as watched in DB.
		/// </summary>
		/// <returns>OK Response with no content</returns>
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[HttpPost("set-video-watched")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> MarkVideoWatched()
		{
			await _advocateService.SetVideoWatched(User.GetId());
			return Response(StatusCodes.Status204NoContent);
		}

		/// <summary>
		/// Start the practice mode for newly joined advocates
		/// </summary>
		/// <returns>OK Response with no content</returns>
		[HttpPost("start-practice")]
		[Authorize(Roles = SolvRoles.Advocate)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> StartPractice()
		{
			await _advocateService.StartPractice(User.GetId());
			return Response();
		}

		/// <summary>
		/// Current advocate is accepting the agreemend for a specific brand.
		/// </summary>
		/// <returns>OK Response with no content</returns>
		[HttpPost("accept-brand-agreement/{brandId}")]
		[Authorize(Roles = SolvRoles.Advocate)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> AcceptBrandAgreement(Guid brandId)
		{
			await _advocateService.AcceptBrandAgreement(User.GetId(), brandId);
			return Response();
		}

		/// <summary>
		/// Current advocate is agreeing to the contract for a specific brand.
		/// </summary>
		/// <returns>OK Response with no content</returns>
		[HttpPost("agree-to-contract/{brandId}")]
		[Authorize(Roles = SolvRoles.Advocate)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> AgreeToContract(Guid brandId)
		{
			await _advocateService.AgreeToContract(User.GetId(), brandId);
			return Response();
		}

		/// <summary>
		/// Enable a brand for the advocate. Allows to get new tickets from it.
		/// </summary>
		/// <returns>OK Response with no content</returns>
		[HttpPost("enable-brand/{brandId}")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> EnabledBrand(Guid brandId)
		{
			await _advocateService.EnableBrand(User.GetId(), brandId);

			return Response();
		}

		/// <summary>
		/// Disable a brand for the advocate. Stops getting new tickets from it.
		/// </summary>
		/// <returns>OK Response with no content</returns>
		[HttpPost("disable-brand/{brandId}")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DisableBrand(Guid brandId)
		{
			await _advocateService.DisableBrand(User.GetId(), brandId);
			return Response();
		}

		/// <summary>
		/// Gets advocate statistics by status.
		/// Role-based - returns data associated with role of user requesting data
		/// </summary>
		[Authorize]
		[HttpGet("statistics/status")]
		[ProducesResponseType(typeof(AdvocateStatisticByStatusModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAdvocateStatisticsByStatus()
		{
			if (User.IsInRole(SolvRoles.Admin))
			{
				var result = await _advocateService.GetStatisticsByStatusForAll();
				return Response(result);
			}

			if (User.IsInRole(SolvRoles.Client))
			{
				var clientId = User.GetId();
				var brandId = await _brandService.GetClientBrandId(clientId);
				var result = await _advocateService.GetStatisticsByStatusForBrand(brandId);
				return Response(result);
			}

			return Forbid();
		}

		/// <summary>
		/// Search and filter the advocate list.
		/// </summary>
		[Authorize]
		[HttpGet("search")]
		[ProducesResponseType(typeof(IPagedList<AdvocateSearchModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> AdvocateSearch([FromQuery] AdvocateSearchCriteriaModel model)
		{
			if (User.IsInRole(SolvRoles.Admin))
			{
				return Response(await _advocateSearchService.Search(model));
			}

			if (User.IsInRole(SolvRoles.Client))
			{
				model.BrandId = await _brandService.GetClientBrandId(User.GetId());
				return Response(await _advocateSearchService.Search(model));
			}

			return Forbid();
		}

		/// <summary>
		/// Returns list of payments of the advocate
		/// </summary>
		[HttpGet("{advocateId}/invoices")]
		[Authorize(Roles = SolvRoles.Advocate)]
		[ProducesResponseType(typeof(IPagedList<AdvocateInvoiceModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAdvocateInvoices(Guid advocateId)
		{
			if (advocateId == User.GetId())
			{
				if (await _featureManager.IsEnabledAsync(nameof(FeatureFlags.InvoicingMicroService)))
				{
					return Response(await _invoicingService.GetAdvocateInvoices(advocateId));
				}
				return Response(await _invoiceService.GetAdvocateInvoiceList(advocateId));
			}
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Returns invoice data
		/// </summary>
		[HttpGet("{advocateId}/invoices/{invoiceId}")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.Admin)]
		[ProducesResponseType(typeof(AdvocateInvoicePrintableModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAdvocateInvoice(Guid advocateId, Guid invoiceId)
		{
			if (User.IsInRole(SolvRoles.Admin))
			{
				var model = await _featureManager.IsEnabledAsync(nameof(FeatureFlags.InvoicingMicroService)) ?
					await _invoicingService.GetAdvocateInvoice(invoiceId) : await _invoiceService.GetAdvocateInvoicePrintable(invoiceId);

				return model != null ? Response(model) : NotFound();
			}

			if (advocateId == User.GetId())
			{
				var model = await _featureManager.IsEnabledAsync(nameof(FeatureFlags.InvoicingMicroService)) ?
					await _invoicingService.GetAdvocateInvoice(advocateId, invoiceId) : await _invoiceService.GetAdvocateInvoicePrintable(invoiceId, advocateId);

				return model != null ? Response(model) : NotFound();
			}

			return Forbid();
		}

		/// <summary>
		/// Returns the induction details of the advocate for the specified brand.
		/// </summary>
		[Authorize(Roles = SolvRoles.Advocate)]
		[HttpGet("{advocateId}/brands/{brandId}/induction")]
		[ProducesResponseType(typeof(AdvocateInductionModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> GetAdvocateInduction(Guid advocateId, Guid brandId)
		{
			if (advocateId != User.GetId())
			{
				return Response(StatusCodes.Status403Forbidden);
			}

			var advocateExists = await _advocateService.ExistsAsync(advocateId, brandId);

			if (!advocateExists)
			{
				NotifyError("Advocate", "", StatusCodes.Status400BadRequest);
			}

			return Response(await _advocateService.GetInduction(advocateId, brandId));
		}

		/// <summary>
		/// Sets the induction section item to viewed.
		/// </summary>
		/// <returns>OK Response with no content</returns>
		[Authorize(Roles = SolvRoles.Advocate)]
		[HttpPost("{advocateId}/brands/{brandId}/induction/{itemId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> MarkInductionItem(Guid advocateId, Guid brandId, Guid itemId)
		{
			if (advocateId != User.GetId())
			{
				return Response(StatusCodes.Status403Forbidden);
			}

			var advocateExists = await _advocateService.ExistsAsync(advocateId, brandId);

			if (!advocateExists)
			{
				NotifyError("Advocate", "", StatusCodes.Status400BadRequest);
			}

			await _advocateService.MarkInductionItem(User.GetId(), brandId, itemId);

			return Response(StatusCodes.Status204NoContent);
		}

		/// <summary>
		/// Save the guideline for Advocate Brand.
		/// </summary>
		/// <returns>OK Response with no content</returns>
		[Microsoft.AspNetCore.Authorization.Authorize(Roles = SolvRoles.Advocate)]
		[HttpPost("{advocateId}/brands/{brandId}/guideline")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> AgreedGuideLine(Guid advocateId, Guid brandId)
		{
			if (advocateId != User.GetId())
			{
				return Response(StatusCodes.Status403Forbidden);
			}

			var advocateExists = await _advocateService.ExistsAsync(advocateId, brandId);

			if (!advocateExists)
			{
				NotifyError("Advocate", "", StatusCodes.Status400BadRequest);
			}

			await _advocateService.SetGuideLine(User.GetId(), brandId);

			return Response(StatusCodes.Status204NoContent);
		}

		/// <summary>
		/// Returns the url to redirect the solvers to, in order to authorize
		/// Solv for receiving payments
		/// </summary>
		[HttpGet("paypal-redirect-url")]
		[Authorize(Roles = SolvRoles.Advocate)]
		[ProducesResponseType(typeof(AdvocatePayPalUrlResponseModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetAdvocatePayPalRedirectUrl()
		{
			var url = await _paymentService.GetPartnerReferralUrl(User.GetId());
			return Response(new AdvocatePayPalUrlResponseModel { Url = url });
		}

		/// <summary>
		/// Request the latest status from PayPal server,
		/// update Solv and return the data.
		/// </summary>
		[HttpPost("update-paypal-status")]
		[Authorize(Roles = SolvRoles.Advocate)]
		[ProducesResponseType(typeof(AdvocateUpdatePaymentMethodModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> UpdatePayPalAccountStatus()
		{
			var (paymentMethodSetup, emailVerified, paymentAccountId) = await _advocateService.UpdatePaymentMethodStatus(User.GetId());
			return Response(new AdvocateUpdatePaymentMethodModel { PaymentMethodSetup = paymentMethodSetup, PaymentEmailVerified = emailVerified, PaymentAccountId = paymentAccountId });
		}

		/// <summary>
		/// Post quiz answers for the brand
		/// </summary>
		[HttpPost("{advocateId}/brands/{brandId}/quiz")]
		[Authorize(Roles = SolvRoles.Advocate)]
		[ProducesResponseType(typeof(QuizResultModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> PostQuiz(Guid advocateId, Guid brandId, [FromBody] IEnumerable<QuizAnswerModel> answers)
		{
			if (advocateId == User.GetId())
			{
				var quiz = await _quizService.Get(brandId);
				var quizReview = _quizService.GetQuizReview(quiz, answers);
				var success = await _quizService.SaveAttempt(advocateId, quizReview.IsQuizPassed, quiz.Id, answers);

				if (quizReview.IsQuizPassed && success)
				{
					await _advocateService.MarkQuizAsPassed(advocateId, brandId);
				}

				if (quizReview.IsQuizPassed)
				{
					var model = new QuizResultModel(quizReview.IsQuizPassed, quizReview.IsQuizPassed ? quiz.SuccessMessage : quiz.FailureMessage);
					return Response(model);
				}
				else
				{
					var model = new QuizResultModel(quizReview.IsQuizPassed, quizReview.IsQuizPassed ? quiz.SuccessMessage : quiz.FailureMessage, quizReview.QuizQuestionReview);
					return Response(model);
				}
			}

			return Forbid();
		}

		/// <summary>
		/// Assign specific brands to an advocate.
		/// </summary>
		[HttpPost("{advocateId}/brands")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> AssignBrands(Guid advocateId, [FromBody] List<Guid> brandIds, [FromQuery] bool authorised = false)
		{
			await _advocateService.SetBrands(advocateId, brandIds, authorised);
			return Response();
		}

		/// <summary>
		/// Create new super solver accounts
		/// </summary>
		[HttpPost("super-solvers")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(typeof(IEnumerable<AdvocateModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CreateSuperSolverAccounts([FromBody] IEnumerable<CreateSuperSolverModel> request)
		{
			var result = new List<AdvocateModel>();

			foreach (var model in request)
			{
				var accountId = await _advocateService.CreateSuperSolver(model.FirstName, model.LastName, model.Email, model.CountryCode, model.Phone, model.Password);
				if (accountId != null)
				{
					result.Add(await _advocateService.FindAsync(accountId.Value));
				}
			}

			return Response(result);
		}

		/// <summary>
		/// Gets the generated JWT Web SDK Token for current Solver
		/// </summary>
		[HttpGet("{advocateId}/identity/token")]
		[Authorize(Roles = SolvRoles.Advocate)]
		[ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> GetIdentityToken(
			[FromRoute] Guid advocateId,
			[FromServices] IIdentityVerificationService identityVerificationService
			)
		{
			if (User.GetId() == advocateId)
			{
				var token = await identityVerificationService.GenerateSdkToken(advocateId);

				return token == null ? BadRequest("Invalid applicant identity") : Response(new TokenModel { Token = token });
			}

			return Forbid();
		}

		/// <summary>
		/// Starts identity check process for the advocate
		/// </summary>
		[HttpGet("{advocateId}/identity/check")]
		[Authorize(Roles = SolvRoles.Advocate)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> StartIdentityCheck(
			[FromRoute] Guid advocateId,
			[FromServices] IIdentityVerificationService identityVerificationService
			)
		{
			if (User.GetId() == advocateId)
			{
				await identityVerificationService.CreateCheck(advocateId);
				return Response();
			}

			return Forbid();
		}

		/// <summary>
		/// Receives identity verification result notification
		/// </summary>
		[HttpPost("identity/webhook")]
		[AllowAnonymous]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
		public async Task<IActionResult> IdentityNotificationWebhook([FromServices] IIdentityVerificationService identityVerificationService)
		{
			try
			{
				await identityVerificationService.ConsumeIdentityCheckWebhook(Request);
			}
			catch (ApiException)
			{
				return UnprocessableEntity();
			}

			return NoContent();
		}

		/// <summary>
		/// Returns periods statistics of closed tickets for specified advocate
		/// </summary>
		[HttpGet("{advocateId}/statistics/periods")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(typeof(AdvocateStatisticPeriodSummaryModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> GetAdvocateStatisticsForPeriods([FromRoute] Guid advocateId)
		{
			if (User.GetId() == advocateId)
			{
				var result = await _ticketService.GetStatisticsPeriodPackage(advocateId);
				return Response(result);
			}

			return Forbid();
		}

		/// <summary>
		/// Exports all advocates applications.
		/// </summary>
		[FeatureGate(FeatureFlags.ApplicationsExport)]
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpGet("exportCsv")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> ExportToCsv()
		{
			var advocateApplications = await _advocateService.GetAllAdvocate();
			return Content(advocateApplications, "text/csv");
		}

		/// <summary>
		/// Returns periods statistics of closed tickets for specified advocate
		/// </summary>
		[HttpGet("{advocateId}/statistic/periods/{period}breakdown")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(typeof(AdvocatePerformanceStatisticPeriodSummaryModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> GetAdvocatePerformanceBreakdownAndSummaryForPeriods(Guid advocateId, string period, [FromQuery] AdvocatePerformanceRequestModel model)
		{
			if (User.GetId() == advocateId)
			{
				var result = await _ticketService.GetAdvocatePerformanceStatisticsPeriod(advocateId, period, model.From.Value, model.BrandIds);
				return Response(result);
			}

			return Forbid();
		}
	}
}