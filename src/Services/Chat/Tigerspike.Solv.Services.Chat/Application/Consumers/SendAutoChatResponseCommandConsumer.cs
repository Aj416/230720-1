using System;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Chat.Application.Services;
using Tigerspike.Solv.Chat.Enums;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Messaging.Chat;
using Tigerspike.Solv.Services.Chat.Application.Models;
using Tigerspike.Solv.Services.Chat.Enums;
using Tigerspike.Solv.Services.Chat.Infrastructure.Repositories;

namespace Tigerspike.Solv.Services.Chat.Application.Consumers
{
	public class SendAutoChatResponseCommandConsumer :
		IConsumer<ISendAutoChatResponseCommand>
	{
		private readonly IMapper _mapper;
		private readonly IMediator _mediator;
		private readonly IChatService _chatService;
		private readonly ILogger<SendAutoChatResponseCommandConsumer> _logger;
		private readonly IChatActionRepository _chatActionRepository;

		public SendAutoChatResponseCommandConsumer(
			IChatService chatService,
			IChatActionRepository chatActionRepository,
			IMapper mapper,
			IMediator mediator,
			ILogger<SendAutoChatResponseCommandConsumer> logger)
		{
			_chatService = chatService;
			_chatActionRepository = chatActionRepository;
			_mapper = mapper;
			_mediator = mediator;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<ISendAutoChatResponseCommand> context)
		{
			if (context.Message.Content.IsNotEmpty())
			{
				await _chatService.AddMessage(context.Message.TicketId, new MessageCreateModel
				{
					AuthorId = context.Message.AuthorId,
					SenderType = (UserType) context.Message.SenderType,
					Message = context.Message.Content,
					RelevantTo = context.Message.RelevantTo.ToList<UserType>(),
					MessageType = context.Message.ActionId == null ? MessageType.Message : MessageType.Action,
					ClientMessageId = Guid.NewGuid(),
					Action = context.Message.ActionId == null
						? null
						: _mapper.Map<ActionModel>(await _chatActionRepository.GetById(context.Message.ActionId.Value)),
				});
			}

			if (context.Message.ResponseType != null)
			{
				_logger.LogDebug($"AutoResponseCompletedEvent for ticket {context.Message.TicketId} response type {context.Message.ResponseType} with value {context.Message.ResponseType.Value}");
				await context.Publish<IChatAutoResponseCompletedEvent>(new { ConversationId = context.Message.TicketId, ResponseType = context.Message.ResponseType.Value});
			}
		}
	}
}
