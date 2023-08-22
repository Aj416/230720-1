using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Messaging.IdentityVerification;
using Tigerspike.Solv.Services.IdentityVerification.Application.Services;

namespace Tigerspike.Solv.Services.IdentityVerification.Controllers
{
	public class HomeController : ApiController
	{
		private readonly IIdentityVerificationService _identityVerificationService;
		private readonly IBus _bus;

		public HomeController(
			IIdentityVerificationService identityVerificationService,
			IBus bus,
			IDomainNotificationHandler notifications,
			IMediatorHandler mediator) : base(notifications,
			mediator)
		{
			_identityVerificationService = identityVerificationService;
			_bus = bus;
		}

		/// <summary>
		/// Generates an sdk token by applicant id.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <param name="applicantId">The applicant identifier.</param>
		/// <param name="firstName">Advocate first name.</param>
		/// <param name="lastName">Advocate last name.</param>
		/// <returns></returns>
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpGet("token")]
		public async Task<IActionResult> GenerateSdkToken(Guid advocateId, string applicantId, string firstName, string lastName)
		{
			if (string.IsNullOrEmpty(applicantId))
			{
				applicantId = await _identityVerificationService.CreateApplicant(firstName, lastName);

				// publish integration event
				await _bus.Publish<IIdentityCreatedEvent>(new
				{
					AdvocateId = advocateId,
					ApplicantId = applicantId,
					Timestamp = DateTime.UtcNow
				});
			}

			return Response(await _identityVerificationService.GenerateSdkToken(applicantId));
		}

		/// <summary>
		/// Receives identity verification result notification
		/// </summary>
		[HttpPost("webhook")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
		public async Task<IActionResult> IdentityNotificationWebhook()
		{
			return await _identityVerificationService.ConsumeWebhook(Request)
				? Response()
				: UnprocessableEntity();
		}
	}
}