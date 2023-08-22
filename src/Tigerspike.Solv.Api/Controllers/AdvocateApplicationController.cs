using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Tigerspike.Solv.Api.Configuration;
using Tigerspike.Solv.Api.Models;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Admin;
using Tigerspike.Solv.Application.Models.Profile;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Core.Bus;
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
using Tigerspike.Solv.Search.Interfaces;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// Advocate Application controller
	/// </summary>
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/advocateApplications")]
	public class AdvocateApplicationController : ApiController
	{
		private readonly IAdvocateApplicationService _advocateApplicationService;
		private readonly IBrandService _brandService;
		private readonly ISearchService<AdvocateApplicationSearchModel> _advocateApplicationSearchService;
		private readonly IMapper _mapper;

		/// <summary>
		/// AdvocateApplication controller constructor
		/// </summary>
		public AdvocateApplicationController(
			IAdvocateApplicationService advocateApplicationService,
			IBrandService brandService,
			ISearchService<AdvocateApplicationSearchModel> advocateApplicationSearchService,
			IMapper mapper,
			IDomainNotificationHandler notificationHandler,
			IMediatorHandler mediator) : base(notificationHandler, mediator)
		{
			_advocateApplicationService = advocateApplicationService;
			_advocateApplicationSearchService = advocateApplicationSearchService ??
				throw new ArgumentNullException(nameof(advocateApplicationSearchService));
			_brandService = brandService;
			_mapper = mapper;
		}

		/// <summary>
		/// Gets Questions to present on the UI
		/// </summary>
		/// <returns></returns>
		[HttpGet("questions")]
		[ProducesResponseType(typeof(ProfileQuestionsModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetQuestions()
		{
			var questions = await _advocateApplicationService.GetAllEnabledQuestions();

			if (questions == null)
			{
				return NotFound();
			}

			return Response(StatusCodes.Status200OK, questions);
		}

		/// <summary>
		/// Gets Questions to present on the UI
		/// </summary>
		/// <returns></returns>
		[HttpGet("profile-questions")]
		[FeatureGate(FeatureFlags.NewProfile)]
		[ProducesResponseType(typeof(ProfileQuestionsModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Authorize(Roles = SolvRoles.Advocate)]
		public async Task<IActionResult> GetProfileQuestions()
		{
			var questions = await _advocateApplicationService.GetAllEnabledQuestions(true);

			if (questions == null)
			{
				return NotFound();
			}

			return Response(StatusCodes.Status200OK, questions);
		}

		/// <summary>
		/// Submits answers for processing
		/// </summary>
		/// <param name="answers"></param>
		/// <returns></returns>
		[HttpPost("submit-answers")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
		public async Task<IActionResult> SubmitAnswers(ProfileAnswerModel answers)
		{
			var validationErrors = await _advocateApplicationService.ValidateAnswers(answers);

			if (validationErrors.Any())
			{
				return UnprocessableEntity(validationErrors);
			}

			await _advocateApplicationService.SubmitAnswers(answers);

			return Response(StatusCodes.Status200OK);
		}

		/// <summary>
		/// Submit profile answer for a qiven question.
		/// </summary>
		/// <param name="applicationId">Advocate Application Id.</param>
		/// <param name="model">ApplicationAnswerModel object.</param>
		/// <returns></returns>
		[HttpPost("{applicationId}/question")]
		[FeatureGate(FeatureFlags.NewProfile)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[Authorize(Roles = SolvRoles.Advocate)]
		public async Task<IActionResult> SubmitProfileAnswer(Guid applicationId, [FromBody] ApplicationAnswerModel model)
		{
			if (applicationId != User.GetId())
			{
				return Response(StatusCodes.Status403Forbidden);
			}

			var isValid = await _advocateApplicationService.ValidateProfile(model);

			if (!isValid)
			{
				return Response();
			}

			await _advocateApplicationService.SubmitProfileAnswers(applicationId, model);

			return Response();
		}

		[HttpGet("{applicationId}/question")]
		[FeatureGate(FeatureFlags.NewProfile)]
		[ProducesResponseType(typeof(IEnumerable<AdvocateApplicationProfileModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Authorize(Roles = SolvRoles.Advocate)]
		public async Task<IActionResult> GetProfileAnswers(Guid applicationId)
		{
			if (applicationId != User.GetId())
			{
				return Response(StatusCodes.Status403Forbidden);
			}

			var applicationAnswer = await _advocateApplicationService.GetProfileAnswers(applicationId);
			return applicationAnswer.Any() ? Response(applicationAnswer) : NotFound();
		}

		/// <summary>
		/// Assign advocate application to brands
		/// </summary>
		/// <param name="advocateApplicationId">The advocate application id</param>
		/// <param name="brandIds">The list of brands to assign advocate application to</param>
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpPost("{advocateApplicationId}/brands")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> AssignToBrands(Guid advocateApplicationId, [FromBody] Guid[] brandIds)
		{
			await _brandService.Assign(brandIds, new[] { advocateApplicationId });
			return Response();
		}

		/// <summary>
		/// Get advocate application to brands assignment
		/// </summary>
		/// <param name="advocateApplicationId">The advocate application id</param>
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpGet("{advocateApplicationId}/brands")]
		[ProducesResponseType(typeof(IEnumerable<BrandInviteModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetBrandsAssignment(Guid advocateApplicationId)
		{
			var result = await _advocateApplicationService.GetBrandAssignments(advocateApplicationId);
			return Response(result);
		}

		/// <summary>
		/// Bulk sends an invitation email to the candidates according to the details of their application.
		/// </summary>
		/// <returns>When successful, returns 200 OK. Otherwise, 400 Bad Request.</returns>
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpPost("invite")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> BulkInviteAdvocateFromApplication([FromBody] InviteModel model)
		{
			await _brandService.Assign(model.Brands.ToArray(), model.Items.ToArray());
			await _advocateApplicationService.InviteAdvocateFromApplication(model.Items);
			return Response();
		}

		/// <summary>
		/// Sends an invitation email to the candidate according to the details of their application.
		/// </summary>
		/// <returns>When successful, returns 200 OK. Otherwise, 400 Bad Request.</returns>
		[Obsolete("Left for backwards compatibility only. Use bulk /invite endpoint instead")]
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpPost("{id}/invite")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> InviteAdvocateFromApplication(Guid id)
		{
			await _advocateApplicationService.InviteAdvocateFromApplication(new[] { id });
			return Response();
		}

		/// <summary>
		/// Bulks sets the applications as not suitable.
		/// </summary>
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpPost("decline")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> BulkDeclineAdvocateApplication([FromBody] BulkActionModel model)
		{
			await _advocateApplicationService.DeclineAdvocateApplication(model.Items);
			return Response();
		}

		/// <summary>
		/// Sets the applications as not suitable.
		/// </summary>
		[Obsolete("Left for backwards compatibility only. Use bulk /decline endpoint instead")]
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpPost("{id}/decline")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeclineAdvocateApplication(Guid id)
		{
			await _advocateApplicationService.DeclineAdvocateApplication(new[] { id });
			return Response();
		}

		/// <summary>
		/// Sends a confirmation of deletion email to advocate applicant.
		/// </summary>
		/// <remarks>POST v1/advocateApplications/send-delete</remarks>
		/// <param name="sendDeleteRequest">The send deletion email request.</param>
		/// <param name="recaptchaApiClient">Recaptcha API client</param>
		/// <returns>Always returns 200 OK.</returns>
		/// <response code="200">Deleted an advocates application if it existed.</response>
		[AllowAnonymous]
		[HttpPost("send-delete")]
		[EnableCors(CorsConfiguration.MarketingSitePolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> SendDeleteAdvocateApplication(SendDeleteRequestModel sendDeleteRequest,
			[FromServices] IRecaptchaApiClient recaptchaApiClient)
		{
			// Validate submitted captcha
			if (!await recaptchaApiClient.ValidateCaptcha(sendDeleteRequest.GoogleRecaptchaResponse))
			{
				return BadRequest();
			}

			var success =
				await _advocateApplicationService.SendDeleteAdvocateApplication(
					sendDeleteRequest.Email.ToLowerInvariant());

			return Response(success ? StatusCodes.Status204NoContent : StatusCodes.Status400BadRequest);
		}

		/// <summary>
		/// Deletes an advocates application.
		/// </summary>
		/// <remarks>GET v1/advocateApplications/delete</remarks>
		/// <param name="deleteRequest">The deletion request containing email and key.</param>
		/// <returns>Always returns 200 OK.</returns>
		/// <response code="200">Deleted an advocates application if it existed.</response>
		[AllowAnonymous]
		[HttpPost("delete")]
		[EnableCors(CorsConfiguration.MarketingSitePolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> DeleteAdvocateApplication(DeleteRequestModel deleteRequest)
		{
			var success =
				await _advocateApplicationService.DeleteAdvocateApplication(deleteRequest.Email.ToLowerInvariant(),
					deleteRequest.Key);

			return Response(success ? StatusCodes.Status204NoContent : StatusCodes.Status400BadRequest);
		}

		/// <summary>
		/// Exports an advocates application.
		/// </summary>
		/// <remarks>GET v1/advocateApplications/export</remarks>
		/// <param name="exportRequest">The request with the details of the export</param>
		/// <param name="recaptchaApiClient">Recaptcha API client</param>
		/// <returns>A file export of the advocates application data in JSON format.</returns>
		/// <response code="204">
		/// An export of the advocates application data has been successfully sent via email.
		/// </response>
		[AllowAnonymous]
		[HttpPost("export")]
		[EnableCors(CorsConfiguration.MarketingSitePolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> ExportAdvocateApplication(ExportRequestModel exportRequest,
			[FromServices] IRecaptchaApiClient recaptchaApiClient)
		{
			// Validate submitted captcha
			if (!await recaptchaApiClient.ValidateCaptcha(exportRequest.GoogleRecaptchaResponse))
			{
				return BadRequest();
			}

			var success =
				await _advocateApplicationService.ExportAdvocateApplication(exportRequest.Email.ToLowerInvariant());

			return Response(StatusCodes.Status204NoContent);
		}

		/// <summary>
		/// Exports all advocates applications.
		/// </summary>
		[FeatureGate(FeatureFlags.ApplicationsExport)]
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpGet("export")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> ExportAll(string country = null, bool sanitize = true)
		{
			var applications = await _advocateApplicationService.GetAllForExport(country, sanitize);
			return Response(applications);
		}

		/// <summary>
		/// An advocate application submission end point. Used when the user wants to apply to become
		/// an advocate
		/// </summary>
		/// <param name="request">The application details</param>
		/// <param name="recaptchaApiClient">Recaptcha API client</param>
		/// <param name="newProfileService"></param>
		/// <returns>Ok with no content</returns>
		[AllowAnonymous]
		[HttpPost("apply")]
		[EnableCors(CorsConfiguration.MarketingSitePolicy)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status208AlreadyReported)]
		public async Task<IActionResult> Apply(AdvocateApplicationRequestModel request,
			[FromServices] IRecaptchaApiClient recaptchaApiClient, [FromServices] INewProfileService newProfileService)
		{
			// Validate submitted captcha
			if (!await recaptchaApiClient.ValidateCaptcha(request.GoogleRecaptchaResponse))
			{
				return BadRequest();
			}

			//Check New Profile feature
			if (!newProfileService.NewProfileEnable())
			{
				request.DataPolicyCheckbox = "on";
			}

			if (await _advocateApplicationService.IsEmailInUse(request.Email))
			{
				return Response(StatusCodes.Status208AlreadyReported);
			}

			request.Sanitize();

			var application = new AdvocateApplicationModel
			{
				Country = request.Country,
				State = request.State,
				Email = request.Email,
				FirstName = request.FirstName,
				LastName = request.LastName,
				Phone = request.Phone,
				Source = request.Source,
				IsAdult = request.IsAdult,
				MarketingCheckbox = request.MarketingCheckbox,
				Address = request.Address,
				City = request.City,
				ZipCode = request.ZipCode,
				DataPolicyCheckbox = request.DataPolicyCheckbox,
				Password = request.Password
			};

			var applicationId = await _advocateApplicationService.Apply(application);

			return Response(StatusCodes.Status201Created, new { id = applicationId });
		}

		/// <summary>
		/// Validate that the Application Id provided can still be used on the profiling section for
		/// a new Application
		/// </summary>
		/// <remarks>GET /advocateApplications/{id}/validate</remarks>
		/// <returns>Validate Advocate Application</returns>
		[HttpPost("{id}/validate")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> ValidateApplication(Guid id)
		{
			var result = await _advocateApplicationService.Validate(id);
			return result ? Response(StatusCodes.Status204NoContent) : Response(StatusCodes.Status400BadRequest); // generic error code, since this is public endpoint
		}

		/// <summary>
		/// Returns whether email has previously been used
		/// </summary>
		/// <param name="email">Email to validate</param>
		/// <returns></returns>
		[AllowAnonymous]
		[HttpPost("validate-email")]
		[EnableCors(CorsConfiguration.MarketingSitePolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> ValidateEmail(string email)
		{
			var emailUsed = await _advocateApplicationService.IsEmailInUse(email.ToLowerInvariant());

			return Response(emailUsed ? StatusCodes.Status400BadRequest : StatusCodes.Status204NoContent);
		}

		/// <summary>
		/// Gets Profile Brands to present on the UI
		/// </summary>
		/// <returns></returns>
		[HttpGet("profile-brands")]
		[ProducesResponseType(typeof(IList<ProfileBrandModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetProfileBrands()
		{
			var questions = await _advocateApplicationService.GetProfileBrands();

			if (questions == null)
			{
				return NotFound();
			}

			return Response(StatusCodes.Status200OK, questions);
		}

		/// <summary>
		/// Gets all advocate applications
		/// </summary>
		/// <remarks>GET v1/advocateApplications</remarks>
		/// <returns>Gets all advocate applications</returns>
		/// <response code="200">A collection of advocate applications.</response>
		[HttpGet]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(typeof(List<AdvocateApplicationModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAdvocateApplications() => Response(StatusCodes.Status200OK, await _advocateApplicationService.GetAdvocateApplications());

		/// <summary>
		/// Gets an Advocate Application
		/// </summary>
		/// <remarks>GET /advocateApplications/{id}</remarks>
		/// <returns>The advocate application.</returns>
		[HttpGet("{id}")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(AdminAdvocateApplicationModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAdvocateApplication(Guid id)
		{
			var application = await _advocateApplicationService.GetAdvocateApplication(id);

			if (application?.Application == null) //No AdvocateApplication found
			{
				return Response(StatusCodes.Status404NotFound);
			}

			application.Application.Sanitize();

			return Response(StatusCodes.Status200OK, application);
		}

		/// <summary>
		/// Get Advocate Applications which need to be reviewed to present on the UI
		/// </summary>
		/// <returns>Collection of Advocate Applications</returns>
		[HttpGet("to-review")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(typeof(IPagedList<AdvocateApplicationModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAdvocateApplicationsToReview(int pageIndex = 0, int pageSize = 25,
			AdminAdvocateApplicationStatusSortBy sortBy = AdminAdvocateApplicationStatusSortBy.CreatedDate,
			SortOrder sortOrder = SortOrder.Asc)
		{
			var advocateApplications = await _advocateApplicationService.GetAdminAdvocateApplications(AdvocateApplicationStatus.New, pageIndex, pageSize, sortBy, sortOrder);

			return Response(StatusCodes.Status200OK, advocateApplications);
		}

		/// <summary>
		/// Get Advocate Applications which have previously been invited to present on the UI
		/// </summary>
		/// <returns>Collection of Advocate Applications</returns>
		[HttpGet("invited")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(typeof(IPagedList<AdvocateApplicationModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAdvocateApplicationsInvited(int pageIndex = 0, int pageSize = 25,
			AdminAdvocateApplicationStatusSortBy sortBy = AdminAdvocateApplicationStatusSortBy.CreatedDate,
			SortOrder sortOrder = SortOrder.Asc)
		{
			var advocateApplications = await _advocateApplicationService.GetAdminAdvocateApplications(AdvocateApplicationStatus.Invited, pageIndex, pageSize, sortBy, sortOrder);

			return Response(StatusCodes.Status200OK, advocateApplications);
		}

		/// <summary>
		/// Get Advocate Applications which are not suitable to present on the UI
		/// </summary>
		/// <returns>Collection of Advocate Applications</returns>
		[HttpGet("not-suitable")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(typeof(IPagedList<AdvocateApplicationModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAdvocateApplicationsNotSuitable(int pageIndex = 0, int pageSize = 25,
			AdminAdvocateApplicationStatusSortBy sortBy = AdminAdvocateApplicationStatusSortBy.CreatedDate,
			SortOrder sortOrder = SortOrder.Asc)
		{
			var advocateApplications = await _advocateApplicationService.GetAdminAdvocateApplications(AdvocateApplicationStatus.NotSuitable, pageIndex, pageSize, sortBy, sortOrder);

			return Response(StatusCodes.Status200OK, advocateApplications);
		}

		/// <summary>
		/// Search and filter the advocate application list.
		/// </summary>
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpGet("search")]
		[ProducesResponseType(typeof(IPagedList<AdvocateApplicationSearchModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> AdvocateApplicationsSearch([FromQuery] AdvocateApplicationSearchCriteriaModel model)
		{
			return Response(await _advocateApplicationSearchService.Search(model));
		}

		/// <summary>
		/// Bulk create advocate applications
		/// </summary>
		/// <param name="request">Input parameters</param>
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpPost("bulk")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> BulkCreate(BulkCreateAdvocateApplicationModel request)
		{
			var success = new Dictionary<string, Guid>();
			var failure = new Dictionary<string, string>();
			foreach (var app in request.Applications)
			{
				app.Sanitize();
				app.IsAdult = "on"; // force consent when inserting in bulk
				app.MarketingCheckbox = "on"; // force consent when inserting in bulk
				app.InternalAgent = request.InternalAgent;
				app.Source = request.Source;

				var model = _mapper.Map<AdvocateApplicationModel>(app);
				if (await _advocateApplicationService.IsEmailInUse(model.Email) == false)
				{
					var id = await _advocateApplicationService.Apply(app);

					if (id.HasValue)
					{
						success.Add(model.Email, id.Value);
					}
				}
				else
				{
					failure.TryAdd(model.Email, "Email is already registered");
				}
			}

			if (request.Brands.Any() && success.Any())
			{
				await _brandService.Assign(request.Brands.ToArray(), success.Values.ToArray());
				await _advocateApplicationService.InviteAdvocateFromApplication(success.Values);
			}

			return Response(new { success, failure });
		}

		/// <summary>
		/// Update Advocate application table with all the super solver data which create through API end point
		/// </summary>
		[HttpPost("retrospective-update-supersolver-in-advocateapplication")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> UpdateSuperSolver()
		{
			await _advocateApplicationService.UpdateAdvocateApplicationwithSuperSolver();
			return Response();
		}

		// <summary>
		//Exports all Candidates.
		// </summary>
		[FeatureGate(FeatureFlags.ApplicationsExport)]
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpGet("exportCandidates")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> ExportAllCandidates([FromQuery] AdvocateApplicationExportRequestModel model)
		{
			var applications = await _advocateApplicationService.GetAllForExport(model.From.Value, model.To.Value);
			return Content(applications, "text/csv");
		}
	}
}