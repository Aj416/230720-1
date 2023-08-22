using System;
using System.Collections.Generic;
using Tigerspike.Solv.Chat.Domain;

namespace Tigerspike.Solv.Chat.Infrastructure.Repositories
{
	public interface IMessageWhitelistRepository
	{
		/// <summary>
		/// Gets the whitelisted messages list based on the specified brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <returns>The list of whitelisted messages for the brand.</returns>
		List<MessageWhitelist> GetList(Guid brandId);

		/// <summary>
		/// Adds or updates the item in the store.
		/// </summary>
		/// <param name="message">The whitelisted message to add</param>
		/// <returns>The created or updated message.</returns>
		MessageWhitelist AddOrUpdateMessage(MessageWhitelist message);

		/// <summary>
		/// Deletes the item based on the brand id and message id.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <param name="messageId">The item range key.</param>
		/// <returns></returns>
		void DeleteMessage(Guid brandId, string messageId);
	}
}