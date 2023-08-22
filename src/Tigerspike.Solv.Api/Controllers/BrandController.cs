using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Tigerspike.Solv.Api.Configuration;
using Tigerspike.Solv.Api.Extensions;
using Tigerspike.Solv.Api.Models.Brand;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Brand;
using Tigerspike.Solv.Application.Models.Client;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Enums;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// Brand Controller
	/// </summary>
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/brands")]
	public class BrandController : ApiController
	{
		private readonly IFeatureManager _featureManager;
		private readonly IBrandService _brandService;
		private readonly IClientService _clientService;
		private readonly IInvoicingService _invoicingService;
		private readonly IInvoiceService _invoiceService;
		private readonly IQuizService _quizService;
		private readonly IApiKeyRepository _apiKeyRepository;
		private readonly ILogger<BrandController> _logger;
		private readonly StorageOptions _storageOptions;


		/// <summary>
		/// Brand Constructor
		/// </summary>
		public BrandController(
			IFeatureManager featureManager,
			IBrandService brandService,
			IClientService clientService,
			IInvoicingService invoicingService,
			IInvoiceService invoiceService,
			IQuizService quizService,
			IDomainNotificationHandler notifications,
			IMediatorHandler mediator,
			IApiKeyRepository apiKeyRepository,
			IOptions<StorageOptions> storageOptions,
			ILogger<BrandController> logger) : base(notifications, mediator)
		{
			_featureManager = featureManager;
			_brandService = brandService ??
							throw new ArgumentNullException(nameof(brandService));
			_clientService = clientService;
			_invoicingService = invoicingService;
			_invoiceService = invoiceService;
			_quizService = quizService;
			_apiKeyRepository = apiKeyRepository;
			_logger = logger;
			_storageOptions = storageOptions.Value;
		}

		/// <summary>
		/// Returns brand details
		/// </summary>
		/// <param name="brandId">Id of the brand</param>
		/// <returns>Brand details</returns>
		[Authorize(Roles = SolvRoles.Advocate + ", " + SolvRoles.Customer)]
		[HttpGet("{brandId}")]
		[ProducesResponseType(typeof(BrandModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetBrand(Guid brandId)
		{
			return User.IsInRole(SolvRoles.Customer)
				? Response(await _brandService.GetPublicProfile(brandId))
				: Response(await _brandService.Get(brandId));
		}

		/// <summary>
		/// Returns all brands in the system
		/// </summary>
		/// <returns>Brand list with details</returns>
		[HttpGet]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(typeof(List<BrandModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetBrands()
		{
			var result = await _brandService.GetAll(false);

			return result != null ? Response(result) : NotFound();
		}

		/// <summary>
		/// Gets current pricing for the brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		[Authorize(Roles = SolvRoles.Client)]
		[HttpGet("{brandId}/pricing")]
		[ProducesResponseType(typeof(PricingModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetTicketPrice(Guid brandId)
		{
			var brand = await _brandService.GetClientBrand(User.GetId());
			var model = new PricingModel
			{
				TicketPrice = brand.TicketPrice,
				FeePercentage = brand.FeePercentage,
				Fee = _brandService.CalculateTicketFee(brand.TicketPrice, brand.FeePercentage),
			};

			return brand?.Id == brandId ? Response(model) : Forbid();
		}

		/// <summary>
		/// Sets current ticket price for the brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="ticketPrice">New ticket price</param>
		[Authorize(Roles = SolvRoles.Client)]
		[HttpPost("{brandId}/pricing")]
		[ProducesResponseType(typeof(PricingModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> SetTicketPrice(Guid brandId, [FromBody] decimal ticketPrice)
		{
			var clientBrand = await _brandService.GetClientBrandId(User.GetId());
			if (clientBrand == brandId)
			{
				await _brandService.SetTicketPrice(brandId, ticketPrice, User.GetId());
				return await GetTicketPrice(brandId);
			}
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Sets contract details for the brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="model">The brand contract details</param>
		[Authorize(Roles = SolvRoles.Client)]
		[HttpPost("{brandId}/contract")]
		[ProducesResponseType(typeof(BrandAssetModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> SetContract(Guid brandId, [FromBody] BrandContractModel model)
		{
			var clientBrand = await _brandService.GetClientBrandId(User.GetId());
			if (clientBrand == brandId)
			{
				var contractUrl = await _brandService.SetContract(brandId, model);
				return Response(new BrandAssetModel(contractUrl));
			}
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Returns list of brand invoices (paged)
		/// </summary>
		[HttpGet("{brandId}/invoices")]
		[Authorize(Roles = SolvRoles.Client)]
		[ProducesResponseType(typeof(IPagedList<ClientInvoiceModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetBrandInvoices(Guid brandId, [FromQuery] PagedRequestModel pageRequest,
			[FromQuery] SortOrder sortOrder = SortOrder.Desc,
			[FromQuery] InvoiceSortBy sortBy = InvoiceSortBy.CreatedDate)
		{
			if (brandId == await _brandService.GetClientBrandId(User.GetId()))
			{
				if (await _featureManager.IsEnabledAsync(nameof(FeatureFlags.InvoicingMicroService)))
				{
					return Response(await _invoicingService.GetBrandInvoices(brandId, pageRequest, sortOrder, sortBy));
				}
				return Response(await _invoiceService.GetClientInvoiceList(brandId, pageRequest, sortOrder, sortBy));
			}
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Returns invoice data
		/// </summary>
		[HttpGet("{brandId}/invoices/{invoiceId}")]
		[Authorize(Roles = SolvRoles.Client)]
		[ProducesResponseType(typeof(ClientInvoicePrintableModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetBrandInvoice(Guid brandId, Guid invoiceId)
		{
			var clientBrandId = await _brandService.GetClientBrandId(User.GetId());

			if (clientBrandId == brandId)
			{
				var model = await _featureManager.IsEnabledAsync(nameof(FeatureFlags.InvoicingMicroService))
					? await _invoicingService.GetBrandInvoice(brandId, invoiceId) : await _invoiceService.GetClientInvoicePrintable(invoiceId, brandId);

				return model != null ? Response(model) : NotFound();
			}
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Returns the url to redirect the client to, in order to approve on
		/// Solv for receiving payments
		/// </summary>
		[HttpGet("paypal-redirect-url")]
		[Authorize(Roles = SolvRoles.Client)]
		[ProducesResponseType(typeof(BrandPayPalUrlResponseModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetBrandPayPalRedirectUrl()
		{
			var clientBrandId = await _brandService.GetClientBrandId(User.GetId());
			var url = await _brandService.GenerateBillingAgreementUrl(clientBrandId);
			return Response(new BrandPayPalUrlResponseModel { Url = url });
		}

		/// <summary>
		/// Setup payment for a brand after receiving the token from PayPal
		/// </summary>
		/// <param name="setupPaymentModel">Payment Model</param>
		[HttpPost("setup-payment")]
		[Authorize(Roles = SolvRoles.Client)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> SetupPayment([FromBody] BrandSetupPaymentModel setupPaymentModel)
		{
			var clientBrandId = await _brandService.GetClientBrandId(User.GetId());
			await _brandService.SetupPaymentAccount(clientBrandId, setupPaymentModel.BillingAgreementToken);
			return Response(StatusCodes.Status204NoContent);
		}

		/// <summary>
		/// Assign the passed advocate applications list to the selected brand.
		/// </summary>
		[HttpPost("{brandId}/assign")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> AssignBrand(Guid brandId, [FromBody] Guid[] advocateApplicationIds)
		{
			await _brandService.Assign(new[] { brandId }, advocateApplicationIds);
			return Response();
		}

		/// <summary>
		/// Get quiz questions for the brand
		/// </summary>
		[HttpGet("{brandId}/quiz")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.Admin + "," + SolvRoles.Client)]
		[ProducesResponseType(typeof(QuizModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetQuiz(Guid brandId)
		{
			if (User.IsInRole(SolvRoles.Client))
			{
				var clientBrandId = await _brandService.GetClientBrandId(User.GetId());
				if (clientBrandId != brandId)
				{
					return Forbid();
				}
			}

			return Response(await _quizService.GetModel(brandId, !User.IsInRole(SolvRoles.Advocate)));
		}

		/// <summary>
		/// A brand application get in touch end point. Used when the client wants to apply to become a part of platform
		/// </summary>
		/// <param name="request">The application details</param>
		/// <param name="recaptchaApiClient">Recaptcha API client</param>
		/// <returns>Ok with no content</returns>
		[AllowAnonymous]
		[HttpPost("apply")]
		[EnableCors(CorsConfiguration.MarketingSitePolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> ApplyBrand(BrandApplicationModel request,
			[FromServices] IRecaptchaApiClient recaptchaApiClient)
		{
			// Validate submitted captcha
			if (await recaptchaApiClient.ValidateCaptcha(request.GoogleRecaptchaResponse))
			{
				await _brandService.Apply(request, "New client application");
				return Response();
			}
			else
			{
				return BadRequest("Recaptacha validation failed");
			}
		}

		/// <summary>
		/// A brand application get in touch end point. Used when the client wants to find out about free AI assessment
		/// </summary>
		/// <param name="request">The application details</param>
		/// <param name="recaptchaApiClient">Recaptcha API client</param>
		/// <returns>Ok with no content</returns>
		[AllowAnonymous]
		[HttpPost("apply/ai-assessment")]
		[EnableCors(CorsConfiguration.MarketingSitePolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> ApplyAIAssessment(BrandApplicationModel request,
			[FromServices] IRecaptchaApiClient recaptchaApiClient)
		{
			// Validate submitted captcha
			if (await recaptchaApiClient.ValidateCaptcha(request.GoogleRecaptchaResponse))
			{
				await _brandService.Apply(request, "Free AI assessment enquiry");
				return Response();
			}
			else
			{
				return BadRequest("Recaptacha validation failed");
			}
		}

		/// <summary>
		/// Create new brand
		/// </summary>
		[HttpPost]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(typeof(BrandModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CreateBrand([FromBody] CreateBrandModel model)
		{
			var brandId = await _brandService.Create(model);
			return brandId != null ? Response(await _brandService.Get(brandId.Value)) : Response();
		}

		/// <summary>
		/// Insert new Whitelist
		/// </summary>
		[HttpPost("{brandId}/whitelist")]
		[Authorize(Roles = SolvRoles.Admin + ", " + SolvRoles.Client)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> WhitelistPhrase(Guid brandId, [FromBody] string[] whitelistPhrases)
		{
			var lst = await _brandService.AddWhitelistPhrases(brandId, whitelistPhrases);
			return Response(lst);
		}

		/// <summary>
		/// Delete Phrase in Whitelist corresponding to BrandID
		/// </summary>
		[HttpDelete("{brandId}/whitelist")]
		[Authorize(Roles = SolvRoles.Admin + ", " + SolvRoles.Client)]
		[ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> BlacklistPhrase(Guid brandId, [FromBody] string[] whitelistPhrases)
		{
			var lstremove = await _brandService.DeleteWhitelistPhrase(brandId, whitelistPhrases);
			return Response(lstremove);
		}

		/// <summary>
		/// Create new brand
		/// </summary>
		[HttpPost("{brandId}/clients")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CreateClientAccounts([FromRoute] Guid brandId,
			[FromBody] IEnumerable<CreateClientModel> request)
		{
			foreach (var model in request)
			{
				await _clientService.CreateClient(brandId, model.FirstName, model.LastName, model.Email, model.Phone,
					model.Password);
			}

			return Response();
		}

		/// <summary>
		/// Generate brand api keys
		/// </summary>
		[HttpPost("{brandId}/api-key")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(typeof(ApiKeyModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GenerateApiKeys([FromRoute] Guid brandId)
		{
			var apiKey = await _brandService.GenerateApiKey(brandId);
			await _brandService.CreateApiKey(brandId, apiKey);
			return Response(apiKey);
		}

		/// <summary>
		/// Put user defined api keys for brand
		/// </summary>
		[HttpPut("{brandId}/api-key")]
		[Authorize(Roles = SolvRoles.Admin)]
		[FeatureGate(FeatureFlags.NonSecureApiKeys)]
		[ProducesResponseType(typeof(ApiKeyModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> PutApiKeys([FromRoute] Guid brandId, [FromBody] ApiKeyModel apiKey)
		{
			await _brandService.CreateApiKey(brandId, apiKey);
			return Response(apiKey);
		}

		/// <summary>
		/// Gets Brand public information by application key.
		/// </summary>
		/// <param name="applicationId">The brand id</param>
		[HttpGet("{applicationId}/public")]
		[ProducesResponseType(typeof(BrandPublicModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetBrandPublicInfo(string applicationId)
		{
			var brandId = await _apiKeyRepository.GetBrandIdFromApplicationId(applicationId);

			if (brandId == null)
			{
				return Forbid();
			}

			var brandPublicModel = await _brandService.GetPublicProfile(brandId.Value);
			return Response(brandPublicModel);
		}

		/// <summary>
		/// Get brand induction sections
		/// </summary>
		[HttpGet("{brandId}/induction/sections")]
		[Authorize(Roles = SolvRoles.Admin + "," + SolvRoles.Client)]
		[ProducesResponseType(typeof(BrandInductionModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> GetInductionSections([FromRoute] Guid brandId) => Response(await _brandService.GetBrandInductionModel(brandId));

		/// <summary>
		/// Post new set of brand induction sections
		/// </summary>
		[HttpPost("{brandId}/induction/sections")]
		[Authorize(Roles = SolvRoles.Admin + "," + SolvRoles.Client)]
		[ProducesResponseType(typeof(BrandInductionModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> PostInductionSections([FromRoute] Guid brandId, [FromBody] BrandInductionModel model)
		{
			await _brandService.PostInductionSections(brandId, model);
			return await GetInductionSections(brandId);
		}


		/// <summary>
		/// Post new set of brand induction sections
		/// </summary>
		[HttpPost("{brandId}/assets")]
		[Authorize(Roles = SolvRoles.Admin + "," + SolvRoles.Client)]
		[ProducesResponseType(typeof(BrandAssetModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> PostAsset([FromRoute] Guid brandId, IFormFile file)
		{
			if (file.Length > _storageOptions.BrandAssetsMaxSize)
			{
				return BadRequest(new ProblemDetails() { Detail = "Maximum file size exceeded" });
			}

			if (_storageOptions.BrandAssetsAllowedExtensions.Contains(Path.GetExtension(file.FileName.ToLowerInvariant())) == false)
			{
				return BadRequest(new ProblemDetails() { Detail = "Provided file type is not supported" });
			}

			await using var fileStream = file.OpenReadStream();
			var url = await _brandService.UploadAsset(brandId, fileStream, file.FileName.ToLowerInvariant());
			return Response(new BrandAssetModel(url));
		}

		/// <summary>
		/// Create quiz
		/// </summary>
		[HttpPost("{brandId}/quiz")]
		[Authorize(Roles = SolvRoles.Admin + "," + SolvRoles.Client)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CreateQuiz(Guid brandId, [FromBody] QuizModel quizModel)
		{
			if (User.IsInRole(SolvRoles.Client))
			{
				var clientBrandId = await _brandService.GetClientBrandId(User.GetId());
				if (clientBrandId != brandId)
				{
					return Forbid();
				}
			}
			await _brandService.CreateQuiz(brandId, quizModel);
			return Response();
		}

		/// <summary>
		/// Create Billing Details
		/// </summary>
		[HttpPost("{brandId}/billing-details")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CreateBillingDetails(Guid brandId,
			[FromBody] BillingDetailsModel billingDetailsModel)
		{
			if (await _featureManager.IsEnabledAsync(nameof(FeatureFlags.InvoicingMicroService)))
			{
				await _invoicingService.CreateBillingDetails(brandId, billingDetailsModel);
				;
			}
			else
			{
				await _invoiceService.CreateBillingDetails(brandId, billingDetailsModel);
			}

			return Response();
		}

		/// <summary>
		/// Gets active abandon reasons for brand.
		/// </summary>
		[HttpGet("{brandId}/abandon-reasons")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(typeof(IEnumerable<AbandonReasonModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAbandonReasons(Guid brandId) =>
			Response(await _brandService.GetAbandonReasons(brandId));

		/// <summary>
		/// Insert new abandon reasons
		/// </summary>
		[HttpPost("{brandId}/abandon-reasons")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CreateAbandonReasons(Guid brandId, [FromBody] AbandonReasonModel[] abandonReasons)
		{
			await _brandService.CreateAbandonReasons(brandId, abandonReasons);
			return Response();
		}

		/// <summary>
		/// Insert new tags
		/// </summary>
		[HttpPost("create-tags")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CreateTags([FromBody] CreateTagModel[] tags)
		{
			await _brandService.CreateTags(tags);
			return Response();
		}

		/// <summary>
		/// Gets active tags for brand.
		/// </summary>
		[HttpGet("{brandId}/tags")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver + "," + SolvRoles.Client)]
		[ProducesResponseType(typeof(IEnumerable<TagModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetTags(Guid brandId)
		{
			TicketLevel? userLevel = null;

			if (User.IsInRole(SolvRoles.Advocate) || User.IsInRole(SolvRoles.SuperSolver))
			{
				userLevel = User.GetLevel();
			}

			return Response(await _brandService.GetTags(brandId, true, userLevel));
		}

		/// <summary>
		/// Gets active categories for brand.
		/// </summary>
		[HttpGet("{brandId}/categories")]
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver + "," + SolvRoles.Client)]
		[ProducesResponseType(typeof(IEnumerable<CategoryModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetCategories(Guid brandId) => Response(await _brandService.GetCategories(brandId));

		/// <summary>
		/// Insert new categories
		/// </summary>
		[HttpPost("{brandId}/categories")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CreateCategories(Guid brandId, [FromBody] CreateCategoryModel categories)
		{
			await _brandService.CreateCategories(categories, brandId);
			return Response();
		}
	}
}