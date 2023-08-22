using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Messaging.Chat;
using Tigerspike.Solv.Services.Chat.Application.IntegrationEvents;

namespace Tigerspike.Solv.Services.Chat.Application.Events
{
	public class PublishIntegrationEventsEventHandler :
		INotificationHandler<MessageAddedEvent>,
		INotificationHandler<MessageDeletedEvent>,
		INotificationHandler<MessageUpdatedEvent>
	{
		private readonly ILogger<PublishIntegrationEventsEventHandler> _logger;
		private readonly IBus _bus;
		private readonly IMapper _mapper;

		public PublishIntegrationEventsEventHandler(
			IMapper mapper,
			IBus bus,
			ILogger<PublishIntegrationEventsEventHandler> logger)
		{
			_mapper = mapper;
			_bus = bus;
			_logger = logger;
		}

		public async Task Handle(MessageAddedEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Publishing an IChatMessageAddedEvent for a new message {0} {1} for conversation {2}",
				notification.Message.Message, notification.Message.Type, notification.ConversationId);

			// Publish integration event
			await _bus.Publish<IChatMessageAddedEvent>(new ChatMessageAddedEvent
			{
				Id = notification.Message.Id,
				ClientMessageId = notification.Message.ClientMessageId,
				ConversationId = notification.Message.ConversationId,
				ThreadId = notification.Message.ThreadId,
				CustomerId = notification.CustomerId,
				AdvocateId = notification.AdvocateId,
				BrandId = notification.BrandId,
				AuthorId = notification.Message.AuthorId,
				AuthorFirstName = notification.Message.UserFirstName,
				CreatedDate = notification.Message.CreatedDate,
				SenderType = notification.Message.SenderType,
				Message = notification.Message.Message,
				MessageType = notification.Message.Type,
				RelevantTo = notification.Message.RelevantTo,
				Action = _mapper.Map<IChatAction>(notification.Message.Action)
			}, cancellationToken);
		}

		public async Task Handle(MessageDeletedEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Publishing an IChatMessageDeletedEvent for message {0} for conversation {2}",
				notification.MessageId, notification.ConversationId);

			// Publish integration event
			await _bus.Publish<IChatMessageDeletedEvent>(new ChatMessageDeletedEvent
			{
				ConversationId = notification.ConversationId, MessageId = notification.MessageId,
				CustomerId = notification.CustomerId,
				AdvocateId = notification.AdvocateId,
			}, cancellationToken);
		}

		public async Task Handle(MessageUpdatedEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Publishing an IChatMessageUpdatedEvent {0} for conversation {1}",
				notification.Message.Message, notification.ConversationId);

			// Publish integration event
			await _bus.Publish<IChatMessageUpdatedEvent>(new ChatMessageUpdatedEvent
			{
				Id = notification.Message.Id,
				ConversationId = notification.Message.ConversationId, ThreadId = notification.Message.ThreadId,
				CustomerId = notification.CustomerId,
				AdvocateId = notification.AdvocateId,
				AuthorId = notification.Message.AuthorId,
				AuthorFirstName = notification.Message.UserFirstName,
				CreatedDate = notification.Message.CreatedDate,
				SenderType = notification.Message.SenderType,
				Message = notification.Message.Message, MessageType = notification.Message.Type,
				ClientMessageId = notification.Message.ClientMessageId,
				RelevantTo = notification.Message.RelevantTo,
				Action = _mapper.Map<IChatAction>(notification.Message.Action)
			}, cancellationToken);
		}
	}
}
