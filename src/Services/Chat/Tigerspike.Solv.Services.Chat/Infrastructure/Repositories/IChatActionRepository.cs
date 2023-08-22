using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Services.Chat.Domain;
using Action = Tigerspike.Solv.Chat.Domain.Action;

namespace Tigerspike.Solv.Services.Chat.Infrastructure.Repositories
{
	public interface IChatActionRepository
	{
		/// <summary>
		/// Gets the chat action and details by id.
		/// </summary>
		/// <param name="chatActionId">The chat action id.</param>
		/// <returns>Chat action with options and effects.</returns>
		public Task<Action> GetById(Guid chatActionId);
	}
}