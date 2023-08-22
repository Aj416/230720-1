using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Tigerspike.Solv.Application.Constants;
using Tigerspike.Solv.Application.Models.Chat;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Domain.Commands.Chat;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Commands
{
	public class SendInitialChatMessageCommandHandler  : IRequestHandler<SendInitialChatMessageCommand>
	{
		private readonly IMapper _mapper;
		private readonly IChatService _chatService;

		public SendInitialChatMessageCommandHandler(IMapper mapper, IChatService chatService)
		{
			_mapper = mapper;
			_chatService = chatService;
		}

		/// <summary>
		/// The handler for SendInitialChatMessageCommand
		/// </summary>
		/// <param name="request"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Unit> Handle(SendInitialChatMessageCommand request, CancellationToken cancellationToken)
		{
			//The ticket is brand new.
			await _chatService.CreateConversation(request.TicketId, new ConversationCreateModel
			{
				TicketId = request.TicketId,
				BrandId = request.BrandId,
				CustomerId = request.CustomerId,
				TransportType = (int) request.TransportType,
				ThreadId = request.ThreadId
			});

			var chatMessages = _mapper.Map<List<MessageAddModel>>(request.Messages);
			
			foreach(var message in chatMessages)
			{
				await _chatService.AddMessage(request.TicketId, message);
			}
			return Unit.Value;
		}
	}
}