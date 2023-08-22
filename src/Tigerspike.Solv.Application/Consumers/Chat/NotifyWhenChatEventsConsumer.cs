using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Application.Models.Chat;
using Tigerspike.Solv.Application.SignalR;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Messaging.Chat;

namespace Tigerspike.Solv.Application.Consumers.Chat
{
	public class NotifyWhenChatEventsConsumer :
		IConsumer<IChatMessageAddedEvent>,
		IConsumer<IChatMessageUpdatedEvent>,
		IConsumer<IChatMessageDeletedEvent>
	{
		private readonly IHubContext<ChatHub> _chatHub;
		private readonly IMapper _mapper;
		private ILogger<NotifyWhenChatEventsConsumer> _logger;

		public NotifyWhenChatEventsConsumer(IMapper mapper, IHubContext<ChatHub> chatHub, ILogger<NotifyWhenChatEventsConsumer> logger)
			=> (_mapper, _chatHub, _logger) = (mapper, chatHub, logger);

		public async Task Consume(ConsumeContext<IChatMessageAddedEvent> context)
		{
			var notification = context.Message;

			_logger.LogInformation("Send a notification after receiving a new chat message {0} for ticket {1}",
				notification.Message, notification.ConversationId);

			var message = new MessageModel
			{
				Id = notification.Id,
				ClientMessageId = notification.ClientMessageId,
				ConversationId = notification.ConversationId,
				ThreadId = notification.ThreadId,
				AuthorId = notification.AuthorId,
				Type = (MessageType)notification.MessageType,
				CreatedDate = notification.CreatedDate,
				Message = notification.Message,
				UserFirstName = notification.AuthorFirstName,
				SenderType = (UserType)notification.SenderType,
				RelevantTo = notification.RelevantTo.Select(x => (UserType)x).ToList(),
				Action = _mapper.Map<ActionModel>(notification.Action)
			};

			await SendMessage(notification.CustomerId, notification.AdvocateId, ChatHubConstants.MESSAGE_ADDED, message);
		}

		public async Task Consume(ConsumeContext<IChatMessageUpdatedEvent> context)
		{
			var notification = context.Message;

			_logger.LogInformation("Send a notification after receiving an updated chat message {0} for ticket {1}",
				notification.Message, notification.ConversationId);

			var message = new MessageModel
			{
				Id = notification.Id,
				ConversationId = notification.ConversationId,
				ThreadId = notification.ThreadId,
				AuthorId = notification.AuthorId,
				Type = (MessageType)notification.MessageType,
				CreatedDate = notification.CreatedDate,
				Message = notification.Message,
				UserFirstName = notification.AuthorFirstName,
				SenderType = (UserType)notification.SenderType,
				ClientMessageId = notification.ClientMessageId,
				RelevantTo = notification.RelevantTo.Select(x => (UserType)x).ToList(),
				Action = _mapper.Map<ActionModel>(notification.Action)
			};

			await SendMessage(notification.CustomerId, notification.AdvocateId, ChatHubConstants.MESSAGE_UPDATED, message);
		}

		public async Task Consume(ConsumeContext<IChatMessageDeletedEvent> context)
		{
			var notification = context.Message;

			_logger.LogInformation("Send a notification after a chat message {0} has been deleted for ticket {1}",
				notification.MessageId, notification.ConversationId);

			await _chatHub.Clients.User(notification.CustomerId.ToString())
				.SendAsync(ChatHubConstants.MESSAGE_DELETED, new MessageDeleteModel(notification.MessageId, notification.ConversationId));
		}

		private async Task SendMessage(Guid customerId, Guid? advocateId, string message, object args = null)
		{
			var users = new List<string> {customerId.ToString()};

			if (advocateId != null)
			{
				users.Add(advocateId.ToString());
			}

			await _chatHub.Clients.Users(users)
				.SendAsync(message, args);
		}
	}

	public class MessageDeleteModel
	{
		public Guid Id { get; set; }

		public Guid ConversationId { get; set; }

		public MessageDeleteModel(Guid id, Guid conversationId)
			=> (Id, ConversationId) = (id, conversationId);

	}
}