using System;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Commands
{
	public class DelayChatResponseCommand : IScheduledJob
	{
		public string JobId => $"{nameof(DelayChatResponseCommand)}-{ResponseId}-{TicketId}";

		public Guid ResponseId { get; private set; }
		public Guid TicketId { get; private set; }
		public Guid? AuthorId { get; private set; }
		public int? ResponseType { get; private set; }
		public int SenderType { get; private set; }
		public string Content { get; private set; }
		public string RelevantTo { get; private set; }

		public Guid? ActionId {get; private set; }
		public TicketStatusEnum? StatusOnPosting { get; private set; }

		/// <summary>
		/// The empty constructor for the bus activator
		/// </summary>
		public DelayChatResponseCommand()
		{

		}

		/// <summary>
		/// The short constructor - for message cancelling
		/// </summary>
		public DelayChatResponseCommand(Guid responseId, Guid ticketId)
		{
			ResponseId = responseId;
			TicketId = ticketId;
		}

		/// <summary>
		/// The constructor.
		/// </summary>
		public DelayChatResponseCommand(Guid responseId, Guid ticketId, Guid? authorId, BrandAdvocateResponseType? responseType, UserType senderType, string content, string relevantTo, Guid? actionId, TicketStatusEnum? statusOnPosting)
		{
			ResponseId = responseId;
			TicketId = ticketId;
			AuthorId = authorId;
			ResponseType = (int?)responseType;
			SenderType = (int)senderType;
			Content = content;
			RelevantTo = relevantTo;
			ActionId = actionId;
			StatusOnPosting = statusOnPosting;
		}

	}
}