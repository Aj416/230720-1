using System;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Services.Chat.Enums;

namespace Tigerspike.Solv.Chat.Application.Commands
{
	public class UpdateAdvocateForConversation : Command
	{
		public Guid ConversationId { get; }

		public TicketStatusEnum Status { get; }

		public Guid? AdvocateId { get; }

		public string AdvocateFirstName { get; }

		public decimal? AdvocateCsat { get; }

		public UpdateAdvocateForConversation(Guid conversationId, TicketStatusEnum status,
			Guid? advocateId, string advocateFirstName,
			decimal? advocateCsat)
		{
			ConversationId = conversationId;
			Status = status;
			AdvocateId = advocateId;
			AdvocateFirstName = advocateFirstName;
			AdvocateCsat = advocateCsat;
		}

		public override bool IsValid()
		{
			return ConversationId != Guid.Empty &&
			       (AdvocateId != null || AdvocateFirstName == null && AdvocateCsat == null);
		}
	}
}