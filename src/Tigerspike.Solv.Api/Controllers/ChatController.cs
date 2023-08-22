using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tigerspike.Solv.Api.Models;
using Tigerspike.Solv.Application.Enums;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Chat;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// Chat endpoint
	/// </summary>
	[ApiVersion("1.0")]
	[Route("v{api-version:apiVersion}/chat")]
	public class ChatController : ApiController
	{
		private readonly IChatService _chatService;
		private readonly ISolvAuthorizationService _authorizationService;

		/// <summary>
		/// Chat constructor
		/// </summary>
		public ChatController(
			IChatService chatService,
			ISolvAuthorizationService authorizationService,
			IDomainNotificationHandler notificationHandler,
			IMediatorHandler mediator) : base(notificationHandler, mediator)
		{
			_chatService = chatService;
			_authorizationService = authorizationService;
		}

		/// <summary>
		/// Gets all messages.
		/// </summary>
		/// <param name="ticketId">The ticket identifier.</param>
		/// <returns></returns>
		[Authorize]
		[ProducesResponseType(typeof(List<MessageModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(List<MessageModel>), 200)]
		[HttpGet("{ticketId}/messages")]
		public async Task<IActionResult> GetMessages(Guid ticketId)
		{
			return await _authorizationService.IsAuthorizedToViewTicket(User, ticketId) ?
				Response(await _chatService.GetMessages(ticketId)) :
				Forbid();
		}

		/// <summary>
		/// Adds message to the conversation speicifed.
		/// ChatItemType will be set to ChatItemType.Message.
		/// </summary>
		[Authorize]
		[ProducesResponseType(typeof(MessageModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[HttpPost("{ticketId}/messages")]
		public async Task<IActionResult> AddMessage(Guid ticketId, [FromBody] MessageCreateRequestModel message)
		{
			if (message != null)
			{
				var request = new MessageAddModel
				{
					AuthorId = User.GetId(),
					Message = message.Message,
					ClientMessageId = message.ClientMessageId
				};

				if (User.IsInRole(SolvRoles.Advocate))
				{
					request.SenderType = (int)UserType.Advocate;
				}
				else if (User.IsInRole(SolvRoles.SuperSolver))
				{
					request.SenderType = (int)UserType.SuperSolver;
				}
				else if (User.IsInRole(SolvRoles.Customer))
				{
					request.SenderType = (int)UserType.Customer;
				}
				else
				{
					return Forbid();
				}

				return await _authorizationService.IsAuthorizedToEditTicket(User, SolvOperationEnum.SendMessage, ticketId) ?
					Response(await _chatService.AddMessage(ticketId, request)) :
					Forbid();
			}
			else
			{
				return BadRequest("Message cannot be null or empty.");
			}
		}

		/// <summary>
		/// Post an response to the chat action
		/// </summary>
		[Authorize(Roles = SolvRoles.Customer)]
		[ProducesResponseType(typeof(MessageModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status208AlreadyReported)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[HttpPost("{ticketId}/messages/{messageId}/action")]
		public async Task<IActionResult> PostActionResponse(Guid ticketId, Guid messageId, [FromBody] ActionRequestModel model)
		{
			return await _authorizationService.IsAuthorizedToViewTicket(User, ticketId) ?
				Response(await _chatService.PostActionResponse(ticketId, messageId,User.GetId(), model)) :
				Forbid();
		}

		/// <summary>
		/// Get all the conversations that matched the list of ids passed.
		/// </summary>
		/// <param name="conversationIds">The list of comma separated ids of the tickets (conversations)</param>
		[Authorize]
		[ProducesResponseType(typeof(List<ConversationModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[HttpGet("conversations")]
		public async Task<IActionResult> GetConversation(string conversationIds)
		{
			var ids = new Guid[] { };
			try
			{
				ids = conversationIds.Split(',').Select(Guid.Parse).ToArray();
			}
			catch(NullReferenceException)
			{
				return BadRequest("ConversationIds cannot be empty");
			}
			catch(FormatException)
			{
				return BadRequest("ConversationIds are not valid Guid identifiers");
			}

			return await _authorizationService.IsAuthorizedToViewTicket(User, ids) ?
				Response(await _chatService.GetConversations(conversationIds)) :
				Forbid();
		}

		/// <summary>
		/// Mark the conversation as read (last read date to be the current date)
		/// </summary>
		/// <param name="conversationId">The id of the conversation</param>
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[HttpPost("conversations/{conversationId}/read")]
		public async Task<IActionResult> MarkConversationAsRead(Guid conversationId)
		{
			// Check if the advocate is assigned to the ticket
			if (await _authorizationService.IsAuthorizedToViewTicket(User, conversationId))
			{
				await _chatService.MarkConversationAsRead(conversationId);
				return Response();
			}
			else
			{
				return Forbid();
			}
		}

	}
}