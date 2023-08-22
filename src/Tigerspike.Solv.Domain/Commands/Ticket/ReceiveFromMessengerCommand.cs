using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class ReceiveFromMessengerCommand : Command<Unit>
	{
		public string UserId { get; set; }

		public string DisplayName { get; set; }

		public string ConversationId { get; set; }

		public Guid BrandId { get; set; }

		public string Content { get; set; }

		public string UserEmail
		{
			get
			{
				return $"{UserId}@messenger.com";
			}
		}

		public ReceiveFromMessengerCommand(string userId, string displayName, string conversationId, Guid brandId,
			string content)
		{
			UserId = userId;
			DisplayName = displayName;
			ConversationId = conversationId;
			BrandId = brandId;
			Content = content.Trim();
		}

		public override bool IsValid()
		{
			return !string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(ConversationId) && BrandId != Guid.Empty &&
			       !string.IsNullOrEmpty(Content);
		}
	}
}