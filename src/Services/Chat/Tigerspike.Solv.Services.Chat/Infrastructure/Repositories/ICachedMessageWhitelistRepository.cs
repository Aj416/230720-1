using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Chat.Infrastructure.Repositories
{
	public interface ICachedMessageWhitelistRepository
	{
		/// <summary>
		/// Gets the whitelisted messages list based on the specified brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <returns>The list of whitelisted messages for the brand.</returns>
		List<string> GetList(Guid brandId);

		/// <summary>
		/// Invalidates the cache for the brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		void Invalidate(Guid brandId);
	}
}