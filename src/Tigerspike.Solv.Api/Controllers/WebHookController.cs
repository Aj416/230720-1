using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using Tigerspike.Solv.Api.Authentication.ApiKey;
using Tigerspike.Solv.Api.Models;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Enums;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Commands.Ticket;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// WebHook Controller
	/// </summary>
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/webhooks")]
	public class WebHookController : ApiController
	{
		private readonly IWebHookService _webHookService;
		private readonly ILogger<WebHookController> _logger;
		private readonly IBrandService _brandService;

		/// <summary>
		/// WebHook Constructor
		/// </summary>
		public WebHookController(
			IBrandService brandService,
			IWebHookService webHookService,
			ILogger<WebHookController> logger,
			IDomainNotificationHandler notifications,
			IMediatorHandler mediator
		) : base(notifications, mediator)
		{
			_brandService = brandService;
			_webHookService = webHookService;
			_logger = logger;
		}

		/// <summary>
		/// Registers new webhooks for the client
		/// </summary>
		[FeatureGate(FeatureFlags.WebHooks)]
		[HttpPost]
		[Authorize(Roles = SolvRoles.Client)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CreateSubscription([FromBody] CreateWebHookModel model)
		{
			var brandId = await _brandService.GetClientBrandId(User.GetId());

			await _webHookService.CreateSubscription(brandId, User.GetId(), model);
			return Response(StatusCodes.Status201Created);
		}

		/// <summary>
		/// Unsubscribe existing webhook
		/// </summary>
		[FeatureGate(FeatureFlags.WebHooks)]
		[HttpDelete("{id:guid}")]
		[Authorize(Roles = SolvRoles.Client)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteSubscription(Guid id)
		{
			var brandId = await _brandService.GetClientBrandId(User.GetId());

			await _webHookService.DeleteSubscription(brandId, id);
			return Response();
		}

		/// <summary>
		/// Creates a ticket with the passed information.
		/// </summary>
		/// <returns>
		/// 201 if successful with location header has the url to the ticket for the customer
		/// </returns>
		[FeatureGate(FeatureFlags.WebHooks)]
		[HttpPost("incoming/messenger")]
		[Authorize(AuthenticationSchemes = ApiKeyAuthentication.Scheme)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> ReceiveMessengerWebhook([FromBody] MessengerReceivedModel receivedModel)
		{
			if (receivedModel == null)
			{
				_logger.LogWarning("Received messenger model is null");
				return BadRequest();
			}

			foreach (var modelEvent in receivedModel.Events)
			{
				// TODO: ensure that events are only processed once by maintaining a list of the events in dynamodb

				_logger.LogDebug("Received event {0} with the payload {1}", modelEvent.Type,
					modelEvent.Payload.Message.Content.Text);

				await _mediator.SendCommand(new ReceiveFromMessengerCommand(modelEvent.Payload.Message.Author.UserId,
					modelEvent.Payload.Message.Author.DisplayName, modelEvent.Payload.Conversation.Id,
					User.GetBrandId(), modelEvent.Payload.Message.Content.Text));
			}

			return Response();
		}
	}
}