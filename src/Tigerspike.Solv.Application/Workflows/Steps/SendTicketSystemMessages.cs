using System;
using System.Collections.Generic;
using System.Linq;
using Tigerspike.Solv.Application.Constants;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Chat;
using Tigerspike.Solv.Domain.DTOs;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Tigerspike.Solv.Application.Workflows.Steps
{
	/// <summary>
	/// The workflow step to sent system messages during ticket creation
	/// </summary>
	public class SendTicketSystemMessages : StepBody
	{
		#region Private Properties

		/// <summary>
		/// The mediator handler
		/// </summary>
		private readonly IMediatorHandler _mediator;

		/// <summary>
		/// The brand advocate response config repository
		/// </summary>
		private readonly IBrandAdvocateResponseConfigRepository _brandAdvocateResponseConfigRepository;
		#endregion

		#region Inputs

		/// <summary>
		/// Input workflow model
		/// </summary>
		public SendInitialChatMessageCommand MessagesCommand { get; set; }
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="mediator"></param>
		/// <param name="brandAdvocateResponseConfigRepository"></param>
		public SendTicketSystemMessages(IMediatorHandler mediator, IBrandAdvocateResponseConfigRepository brandAdvocateResponseConfigRepository)
		{
			_mediator = mediator;
			_brandAdvocateResponseConfigRepository = brandAdvocateResponseConfigRepository;
		}
		#endregion

		#region Run(IStepExecutionContext context)

		/// <summary>
		/// The workflow run method
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override ExecutionResult Run(IStepExecutionContext context)
		{
			var ticketCreatedResponse = _brandAdvocateResponseConfigRepository.Get(MessagesCommand.BrandId, BrandAdvocateResponseType.TicketCreatedSystemResponse).Result;
			
			var messages = new List<TicketMessageModel>
			{
				AddSystemMessage(MessageType.StatusChange, ChatMessageConstants.TicketCreated, MessagesCommand.TicketId.ToString()),

				AddSystemMessage(MessageType.SystemMessage, new[] { UserType.Customer }, ticketCreatedResponse.Content),

				AddChatMessage(MessagesCommand.CustomerId, MessageType.Message, UserType.Customer, null, MessagesCommand.Question)
			};

			if (MessagesCommand.IsPractice)
			{
				messages.AddRange((SystemMessage.PractiseTicketMessages.Select(msg =>
					AddSystemMessage(MessageType.Message, msg)).ToArray()));
			}

			MessagesCommand.Messages = messages;
			_mediator.SendCommand(MessagesCommand).Wait();

			return ExecutionResult.Next();
		}
		#endregion

		#region Private Methods

		/// <summary>
		/// A helper method to add system message
		/// </summary>
		/// <param name="ticketId"></param>
		/// <param name="chatType"></param>
		/// <param name="msgText"></param>
		/// <param name="msgTextArgs"></param>
		/// <returns></returns>
		private TicketMessageModel AddSystemMessage(MessageType chatType, string msgText, params string[] msgTextArgs)
			=> AddChatMessage(Guid.Empty, chatType, UserType.System, null, msgText, msgTextArgs);

		/// <summary>
		/// A helper method to add system message
		/// </summary>
		/// <param name="chatType"></param>
		/// <param name="relevantTo"></param>
		/// <param name="msgText"></param>
		/// <param name="msgTextArgs"></param>
		/// <returns></returns>
		private TicketMessageModel AddSystemMessage(MessageType chatType, IEnumerable<UserType> relevantTo,	string msgText,	params string[] msgTextArgs)
			=> AddChatMessage(Guid.Empty, chatType, UserType.System, relevantTo, msgText, msgTextArgs);

		/// <summary>
		/// A helper method to generate a chat message model
		/// </summary>
		/// <param name="authorId"></param>
		/// <param name="chatType"></param>
		/// <param name="senderType"></param>
		/// <param name="relevantTo"></param>
		/// <param name="msgText"></param>
		/// <param name="msgTextArgs"></param>
		/// <returns></returns>
		private TicketMessageModel AddChatMessage(Guid? authorId, MessageType chatType, UserType senderType,
			IEnumerable<UserType> relevantTo, string msgText, params string[] msgTextArgs)
		{
			var message = string.Format(msgText, msgTextArgs);
			
			return new TicketMessageModel
			{
				AuthorId = authorId,
				SenderType = senderType,
				RelevantTo = relevantTo,
				Message = message,
				MessageType = chatType
			};
		}
		#endregion
	}
}
