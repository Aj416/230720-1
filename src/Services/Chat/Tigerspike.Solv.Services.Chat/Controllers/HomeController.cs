using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tigerspike.Solv.Chat.Application.Services;
using Tigerspike.Solv.Chat.Enums;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Services.Chat.Application.Commands;
using Tigerspike.Solv.Services.Chat.Application.Models;
using Tigerspike.Solv.Services.Chat.Enums;

namespace Tigerspike.Solv.Services.Chat.Controllers
{
	/// <summary>
	/// Chat Controller
	/// </summary>
	[ApiVersion("1.0")]
	[Route("")]
	public class HomeController : ApiController
	{
		private readonly IChatService _chatService;

		public HomeController(
			IChatService chatService,
			IDomainNotificationHandler notifications,
			IMediatorHandler mediator) : base(notifications,
			mediator) =>
			_chatService = chatService;

		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpGet]
		public IActionResult Get() => Ok("Solv Chat Service");

		/// <summary>
		/// Gets all messages.
		/// </summary>
		/// <param name="conversationId">The ticket identifier.</param>
		/// <returns></returns>
		[ProducesResponseType(typeof(List<MessageModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpGet("{conversationId}/messages")]
		public async Task<IActionResult> GetMessages(Guid conversationId)
		{
			return conversationId == Guid.Empty ? BadRequest() : Response(await _chatService.GetMessages(conversationId));
		}

		/// <summary>
		/// Get all the conversations that matched the list of ids passed.
		/// </summary>
		/// <param name="conversationIds">The list of comma separated ids of the tickets (conversations)</param>
		[ProducesResponseType(typeof(List<ConversationModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpGet("conversations")]
		public IActionResult GetConversations(string conversationIds)
		{
			Guid[] ids;
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

			return Response(_chatService.GetConversations(ids));
		}

		/// <summary>
		/// Get the conversation by id.
		/// </summary>
		/// <param name="conversationId">The conversation id.</param>
		[ProducesResponseType(typeof(ConversationModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet("{conversationId:guid}")]
		public IActionResult GetConversation(Guid conversationId)
		{
			if (conversationId == Guid.Empty)
			{
				return BadRequest();
			}

			var conversation = _chatService.GetConversation(conversationId);

			if (conversation == null)
			{
				return NotFound();
			}

			return Response(conversation);
		}

		/// <summary>
		/// Adds message to the conversation speicifed.
		/// ChatItemType will be set to ChatItemType.Message.
		/// </summary>
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[HttpGet("{conversationId}/transcript")]
		public async Task<IActionResult> GetTranscript(Guid conversationId)
		{
			return conversationId == Guid.Empty ? BadRequest() : Response(await _chatService.GetTranscript(conversationId));
		}

		/// <summary>
		/// Adds message to the conversation speicifed.
		/// ChatItemType will be set to ChatItemType.Message.
		/// </summary>
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[HttpPost("{conversationId}")]
		public IActionResult CreateConversation(Guid conversationId, [FromBody] ConversationCreateModel conversation)
		{
			_chatService.AddConversation(conversation);

			return Response(StatusCodes.Status201Created);
		}

		/// <summary>
		/// Adds message to the conversation speicifed.
		/// ChatItemType will be set to ChatItemType.Message.
		/// </summary>
		[ProducesResponseType(typeof(MessageModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[HttpPost("{conversationId}/messages")]
		public async Task<IActionResult> AddMessage(Guid conversationId, [FromBody] MessageAddModel message)
		{
			if (message == null)
			{
				return BadRequest("Message cannot be null or empty.");
			}

			var request = new MessageCreateModel
			{
				AuthorId = message.AuthorId,
				Message = message.Message,
				ClientMessageId = message.ClientMessageId,
				SenderType = (UserType)message.SenderType,
				MessageType = message.MessageType.HasValue
					? (MessageType)message.MessageType.Value
					: (UserType)message.SenderType == UserType.System
						? MessageType.SystemMessage
						: MessageType.Message,
				RelevantTo = message.RelevantTo?.Select(t => (UserType)t)
			};

			return Response(await _chatService.AddMessage(conversationId, request));
		}

		/// <summary>
		/// Mark the conversation as read (last read date to be the current date)
		/// </summary>
		/// <param name="conversationId">The id of the conversation</param>
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[HttpPatch("conversations/{conversationId}/read")]
		public async Task<IActionResult> MarkConversationAsRead(Guid conversationId)
		{
			await _chatService.MarkConversationAsRead(conversationId);
			return Response();
		}

		/// <summary>
		/// Post an response to the chat action
		/// </summary>
		[ProducesResponseType(typeof(MessageModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status208AlreadyReported)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[HttpPost("{conversationId}/messages/{messageId}/action")]
		public async Task<IActionResult> PostActionResponse(Guid conversationId, Guid messageId, Guid userId,
			[FromBody] ActionRequestModel model)
		{
			return Response(await _chatService.PostActionResponse(conversationId, messageId, model, userId));
		}

		/// <summary>
		/// Post the solved question to the chat.
		/// </summary>
		[ProducesResponseType(typeof(MessageModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[HttpPost("{conversationId}/actions/solved")]
		public async Task<IActionResult> AddIsTicketSolvedQuestion(Guid conversationId, Guid advocateId)
		{
			return Response(await _chatService.AddIsTicketSolvedQuestion(conversationId, advocateId));
		}

		/// <summary>
		/// Finalize the active action on the conversation.
		/// </summary>
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[HttpPatch("{conversationId}/actions/finalize")]
		public async Task<IActionResult> FinalizeActiveActions(Guid conversationId)
		{
			await _chatService.FinalizeActiveActions(conversationId);

			return Response(StatusCodes.Status200OK);
		}

		/// <summary>
		/// Inserts phrases to whitelist corresponding to a brand. To be moved to the brand service eventually.
		/// </summary>
		[HttpPost("brands/{brandId}/whitelist")]
		[ProducesResponseType(typeof(string[]), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> AddWhitelistPhrases(Guid brandId, [FromBody] string[] whitelistPhrases)
		{
			return Response(StatusCodes.Status201Created,
				await _mediator.SendCommand(new CreateBrandWhitelistPhrasesCommand(brandId, whitelistPhrases)));
		}

		/// <summary>
		/// Deletes phrases from whitelist corresponding to a brand. To be moved to the brand service eventually.
		/// </summary>
		[HttpDelete("brands/{brandId}/whitelist")]
		[ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> RemoveWhitelistPhrases(Guid brandId, [FromBody] string[] whitelistPhrases)
		{
			return Response(
				await _mediator.SendCommand(new DeleteBrandWhitelistPhrasesCommand(brandId, whitelistPhrases)));
		}
	}
}