using System;
using System.Security.Claims;
using Tigerspike.Solv.Chat.Domain;

namespace Tigerspike.Solv.Chat.Infrastructure
{
	public interface IAuthorizationService
	{
		bool CanViewConversation(ClaimsPrincipal user, Guid conversationId);

		bool CanViewConversation(ClaimsPrincipal user, Conversation conversation);

		bool CanJoinConversations(ClaimsPrincipal user, Guid[] conversationIds);
	}
}