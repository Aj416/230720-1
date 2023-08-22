using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Chat.Application.ChatCensor;
using Tigerspike.Solv.Chat.Application.Commands;
using Tigerspike.Solv.Chat.Application.Services;
using Tigerspike.Solv.Chat.Domain;
using Tigerspike.Solv.Chat.Enums;
using Tigerspike.Solv.Chat.Infrastructure.Repositories;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Messaging.Chat;
using Tigerspike.Solv.Services.Chat.Application.Events;
using Tigerspike.Solv.Services.Chat.Application.Models;
using Tigerspike.Solv.Services.Chat.Enums;

namespace Tigerspike.Solv.Services.Chat.Application.Services
{
	public class ChatService : IChatService
	{
		private readonly ILogger<ChatService> _logger;
		private readonly IChatRepository _chatRepository;
		private readonly IMapper _mapper;
		private readonly IMediatorHandler _mediator;
		private readonly IChatCensorContext _chatCensorContext;
		private readonly ICachedMessageWhitelistRepository _cachedMessageWhitelistRepository;
		private readonly IBus _bus;
		private readonly ICachedConversationRepository _cachedConversationRepository;

		public ChatService(
			ICachedMessageWhitelistRepository cachedMessageWhitelistRepository,
			ICachedConversationRepository cachedConversationRepository,
			IChatRepository chatRepository,
			ILogger<ChatService> logger,
			IMapper mapper,
			IBus bus,
			IMediatorHandler mediator,
			IChatCensorContext chatCensorContext
		)
		{
			_cachedMessageWhitelistRepository = cachedMessageWhitelistRepository;
			_cachedConversationRepository = cachedConversationRepository;
			_chatRepository = chatRepository;
			_logger = logger;
			_mapper = mapper;
			_bus = bus;
			_mediator = mediator;
			_chatCensorContext = chatCensorContext;
		}

		private static bool IsMessageTypeFeedBack(Message message, string additionalFeedBack, out bool isFeedBackSubmitted)
		{
			isFeedBackSubmitted = message.Action.Type == (int)ActionType.FeedBack && !string.IsNullOrEmpty(additionalFeedBack) &&
				message.Action.Options.Any(o => o.IsSelected && o.IsSuggested);

			return message.Action.Type == (int)ActionType.FeedBack;
		}

		/// <inheritdoc />
		public async Task<MessageModel> AddMessage(Guid conversationId, MessageCreateModel model)
		{
			if (model == null)
			{
				_logger.LogError("AddItem: message cannot be null.");
				throw new ArgumentNullException(nameof(model));
			}

			var conversation = _chatRepository.GetConversation(conversationId);

			if (conversation != null)
			{
				// Create the message item from the model
				model.AuthorId ??= Guid.Empty;
				var message = new Message(conversationId.ToString(), model.AuthorId.ToString(), model.SenderType,
					model.Message, model.MessageType)
				{
					RelevantTo = _mapper.Map<int[]>(model.RelevantTo),
					Action = _mapper.Map<Solv.Chat.Domain.Action>(model.Action)
				};

				// Censor the chat message when sent by a person
				if (model.MessageType == MessageType.Message)
				{
					var messageWhitelist = _cachedMessageWhitelistRepository.GetList(conversation.BrandId);
					_chatCensorContext.Censor(message, messageWhitelist);
				}

				if (model.Action?.IsBlocking == true)
				{
					await FinalizeActiveActions(conversationId);
				}

				_chatRepository.AddOrUpdateMessage(message);

				var messageModel = _mapper.Map<MessageModel>(message);

				// Set the client id
				messageModel.ClientMessageId = model.ClientMessageId;

				await _mediator.RaiseEvent(new MessageAddedEvent(conversationId, new Guid(conversation.CustomerId),
					conversation.AdvocateId != null ? new Guid(conversation.AdvocateId) : null, conversation.BrandId, messageModel, message.CreatedDate));

				return messageModel;
			}

			return null;
		}

		public async Task<MessageModel> AddSolverResponse(Guid conversationId, Guid advocateId, bool isSuperSolver, string response)
		{
			return await AddMessage(conversationId, new MessageCreateModel
			{
				AuthorId = advocateId,
				SenderType = isSuperSolver ? UserType.SuperSolver : UserType.Advocate,
				Message = response,
				MessageType = MessageType.Message,
				ClientMessageId = Guid.NewGuid()
			});
		}

		/// <inheritdoc />
		public async Task<MessageModel> AddIsTicketSolvedQuestion(Guid conversationId, Guid advocateId)
		{
			return await AddMessage(conversationId, new MessageCreateModel
			{
				AuthorId = advocateId,
				SenderType = UserType.Advocate,
				Message = "Please help us improve.\nDoes this resolve your issue?",
				MessageType = MessageType.Action,
				RelevantTo = new[] { UserType.Customer },
				Action = new ActionModel
				{
					Type = ActionType.IsTicketSolvedQuestion,
					IsBlocking = true,
					Options = new[]
					{
						new ActionOptionModel
						{
							Label = "Yes",
							Value = bool.TrueString,
							IsSuggested = true,
						},
						new ActionOptionModel
						{
							Label = "No, I need to chat more",
							Value = bool.FalseString,
						},
					}
				}
			});
		}

		/// <inheritdoc />
		public async Task<MessageModel> AddCsatAction(Guid conversationId, Guid advocateId)
		{
			return await AddMessage(conversationId, new MessageCreateModel
			{
				AuthorId = advocateId,
				SenderType = UserType.Advocate,
				Message = "How would you rate this support?",
				MessageType = MessageType.Action,
				RelevantTo = new[] { UserType.Customer },
				Action = new ActionModel
				{
					Type = ActionType.CSAT,
					IsBlocking = true,
					Options = Enumerable.Range(1, 5).Select(x => new ActionOptionModel { Value = $"{x}" }),
				}
			});
		}

		/// <inheritdoc />
		public async Task<MessageModel> AddNpsAction(Guid conversationId, Guid advocateId, string brandName)
		{
			return await AddMessage(conversationId, new MessageCreateModel
			{
				AuthorId = advocateId,
				SenderType = UserType.Advocate,
				Message = $"Finally, based on your recent support experience, how likely would you be to recommend {brandName} to a friend, family member or a colleague?",
				MessageType = MessageType.Action,
				RelevantTo = new[] { UserType.Customer },
				Action = new ActionModel
				{
					Type = ActionType.NPS,
					IsBlocking = true,
					Options = Enumerable.Range(0, 11).Select(x => new ActionOptionModel { Label = $"{x}", Value = $"{x}" }),
				}
			});
		}

		/// <inheritdoc />
		public Task<List<MessageModel>> GetMessages(Guid ticketId)
		{
			if (ticketId == Guid.Empty)
			{
				throw new ArgumentException("ticketId is required");
			}

			var chatItemModels = _chatRepository.GetMessages(ticketId);

			return Task.FromResult(_mapper.Map<List<MessageModel>>(chatItemModels));
		}

		/// <inheritdoc />
		public async Task<IEnumerable<MessageModel>> GetMessages(Guid ticketId, UserType perspective) =>
			(await GetMessages(ticketId)).Where(x => x.RelevantTo == null || x.RelevantTo.Count == 0 || x.RelevantTo.Contains((int)perspective));

		/// <inheritdoc />
		public async Task DeleteMessage(Guid conversationId, Guid authorId, Guid messageId, long timestamp)
		{
			var conversation = _chatRepository.GetConversation(conversationId);

			_chatRepository.DeleteMessage(conversationId, Message.GenerateMessageId(timestamp, authorId.ToString()));

			await _mediator.RaiseEvent(new MessageDeletedEvent(conversationId,
				new Guid(conversation.CustomerId),
				conversation.AdvocateId != null ? new Guid(conversation.AdvocateId) : null, messageId));
		}

		/// <inheritdoc />
		public Guid AddConversation(ConversationCreateModel model)
		{
			if (model == null)
			{
				_logger.LogError("conversation: message cannot be null.");
				throw new ArgumentNullException(nameof(model));
			}

			// Create the message item from the model
			var conversation = new Conversation
			{
				Id = model.TicketId.ToString(),
				CreatedDate = DateTime.UtcNow,
				BrandId = model.BrandId,
				CustomerId = model.CustomerId.ToString(),
				TransportType = (TicketTransportType)model.TransportType,
				ThreadId = model.ThreadId
			};

			_chatRepository.AddOrUpdateConversation(conversation);

			return new Guid(conversation.Id);
		}

		/// <inheritdoc />
		public async Task MarkConversationAsRead(Guid conversationId)
		{
			var command = new MarkConversationAsReadCommand(conversationId, DateTime.UtcNow);
			await _mediator.SendCommand(command);
		}

		/// <inheritdoc />
		public List<ConversationModel> GetConversations(params Guid[] conversationIds)
		{
			return _mapper.Map<List<ConversationModel>>(_chatRepository.GetConversations(conversationIds));
		}

		/// <inheritdoc />
		public ConversationModel GetConversation(Guid conversationId)
		{
			return _mapper.Map<ConversationModel>(_cachedConversationRepository.GetConversation(conversationId));
		}

		public async Task<MessageModel> PostActionResponse(Guid conversationId, Guid messageId, ActionRequestModel model, Guid userId)
		{
			var message = _chatRepository.GetMessage(conversationId, messageId);
			var conversation = _chatRepository.GetConversation(conversationId);

			if (message?.Action?.Options == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(nameof(PostActionResponse),
					$"Action {messageId} not found in conversation {conversationId}"));

				return null;
			}

			if (message.Action.IsFinalized == false)
			{
				return await FinalizePostActionResponse(conversationId, messageId, model, userId, message, conversation);
			}

			await _mediator.RaiseEvent(new DomainNotification(nameof(PostActionResponse),
				$"Response for action {messageId} in conversation {conversationId} was already reported",
				(int)HttpStatusCode.AlreadyReported));

			return null;
		}

		private async Task<MessageModel> FinalizePostActionResponse(Guid conversationId, Guid messageId, ActionRequestModel model,
			Guid userId, Message message, Conversation conversation)
		{
			foreach (var opt in message.Action.Options)
			{
				if (model.Options.Contains(opt.Value, StringComparer.InvariantCultureIgnoreCase))
				{
					opt.IsSelected = true;
					message.Action.IsFinalized = true;
				}
			}

			if (IsMessageTypeFeedBack(message, model.AdditionalFeedBack, out var isFeedBackSubmitted))
			{
				if (isFeedBackSubmitted)
				{
					message.Content = model.AdditionalFeedBack;
					message.Type = (int)MessageType.Message;
					message.SenderType = (int)UserType.Customer;
					message.AuthorId = userId.ToString();
					message.RelevantTo = new int[] { (int) UserType.Customer };
					_chatRepository.AddOrUpdateMessage(message);
				}
				else
				{
					_chatRepository.DeleteMessage(conversationId, message.MessageId);
				}
			}
			else
			{
				// store update item
				_chatRepository.AddOrUpdateMessage(message);
			}

			// raise events
			var messageModel = _mapper.Map<MessageModel>(message);
			await _mediator.RaiseEvent(new ActionFinalizedEvent(conversationId, messageModel.Action,
				model.AdditionalFeedBack));

			// Raise integration event

			await _bus.Publish<IChatActionFinalizedEvent>(new
			{
				ConversationId = conversationId,
				Content = model.AdditionalFeedBack,
				Action = new
				{
					Type = (int)messageModel.Action.Type,
					IsFinalized = messageModel.Action.IsFinalized,
					IsBlocking = messageModel.Action.IsBlocking,
					Options = _mapper.Map<List<IChatActionOption>>(messageModel.Action.Options)
				}
			});

			if (message.Action.Type != (int)ActionType.FeedBack)
			{
				return messageModel;
			}

			if (isFeedBackSubmitted)
			{
				await _mediator.RaiseEvent(new MessageUpdatedEvent(conversationId,
					new Guid(conversation.CustomerId),
					conversation.AdvocateId != null ? new Guid(conversation.AdvocateId) : null,
					messageModel));
			}
			else
			{
				await _mediator.RaiseEvent(new MessageDeletedEvent(conversationId,
					new Guid(conversation.CustomerId),
					conversation.AdvocateId != null ? new Guid(conversation.AdvocateId) : null,  messageId));
			}

			return messageModel;

		}

		/// <inheritdoc/>
		public async Task FinalizeActiveActions(Guid conversationId)
		{
			var conversation = _chatRepository.GetConversation(conversationId);
			var actionMessages = _chatRepository.GetActions(conversationId);
			var activeActionMessages = actionMessages.Where(x => x.Action?.IsFinalized == false);

			foreach (var message in activeActionMessages)
			{
				message.Action.IsFinalized = true;
				_chatRepository.AddOrUpdateMessage(message);
				var model = _mapper.Map<MessageModel>(message);

				await _mediator.RaiseEvent(new MessageUpdatedEvent(conversationId,
					new Guid(conversation.CustomerId),
					conversation.AdvocateId != null ? new Guid(conversation.AdvocateId) : null, model));
			}
		}

		/// <inheritdoc />
		public async Task<string> GetTranscript(Guid conversationId)
		{
			var messages = await GetMessages(conversationId, UserType.System);
			var maxAuthorLength = UserType.Customer.ToString().Length;
			var lines = messages.Select(x => new
				{
					Author = ((UserType)x.SenderType).ToDisplay().PadRight(maxAuthorLength),
					Timestamp = x.CreatedDate.ToString("dd-MM-yyyy, HH:mm UTC", CultureInfo.InvariantCulture),
					Message = (x.Message ?? string.Empty).Trim(),
				})
				.Select(x => $"[{x.Author} @ {x.Timestamp}] {x.Message}");
			return string.Join("\n", lines);
		}
	}
}