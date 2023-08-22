using System;
using System.Linq;
using System.Security.Claims;
using Tigerspike.Solv.Chat.Domain;
using Tigerspike.Solv.Chat.Infrastructure.Repositories;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Chat.Infrastructure
{
	public class AuthorizationService : IAuthorizationService
	{
		private readonly IChatRepository _chatRepository;

		public AuthorizationService(IChatRepository chatRepository) => _chatRepository = chatRepository;

		public bool CanViewConversation(ClaimsPrincipal user, Conversation conversation)
		{
			if (user.IsInRole(SolvRoles.Customer))
			{
				return user.HasTokenForTicket(Guid.Parse(conversation.Id));
			}

			if (user.IsInRole(SolvRoles.Advocate) || user.IsInRole(SolvRoles.SuperSolver))
			{
				return conversation.AdvocateId == user.GetId().ToString();
			}

			if (user.IsInRole(SolvRoles.Client))
			{
				return conversation.BrandId == user.GetBrandId();
			}

			if (user.IsInRole(SolvRoles.Admin))
			{
				return true;
			}

			return false;
		}

		public bool CanViewConversation(ClaimsPrincipal user, Guid conversationId)
		{
			if (user == null || conversationId == Guid.Empty)
			{
				return false;
			}

			if (user.IsInRole(SolvRoles.Customer))
			{
				return user.HasTokenForTicket(conversationId);
			}

			if (user.IsInRole(SolvRoles.Advocate) || user.IsInRole(SolvRoles.SuperSolver))
			{
				var conversation = _chatRepository.GetConversation(conversationId);
				return conversation.AdvocateId == user.GetId().ToString();
			}

			if (user.IsInRole(SolvRoles.Client))
			{
				var conversation = _chatRepository.GetConversation(conversationId);
				return conversation.BrandId == user.GetBrandId();
			}

			if (user.IsInRole(SolvRoles.Admin))
			{
				return true;
			}

			return false;
		}

		public bool CanJoinConversations(ClaimsPrincipal user, Guid[] conversationIds)
		{
			if (user.IsInRole(SolvRoles.Customer))
			{
				return conversationIds.Length == 1 && user.HasTokenForTicket(conversationIds.Single());
			}

			var conversations = _chatRepository.GetConversations(conversationIds);

			if (user.IsInRole(SolvRoles.Advocate) || user.IsInRole(SolvRoles.SuperSolver))
			{
				return conversations.All(x => x.AdvocateId == user.GetId().ToString());
			}

			if (user.IsInRole(SolvRoles.Client))
			{
				return conversations.All(x => x.BrandId == user.GetBrandId());
			}

			if (user.IsInRole(SolvRoles.Admin))
			{
				return true;
			}

			return false;
		}
	}
}