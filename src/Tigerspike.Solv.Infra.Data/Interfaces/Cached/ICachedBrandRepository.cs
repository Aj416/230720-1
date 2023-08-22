using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Models;
using Brand = Tigerspike.Solv.Infra.Data.Models.Cached.Brand;

namespace Tigerspike.Solv.Infra.Data.Interfaces.Cached
{
	public interface ICachedBrandRepository
	{
		/// <summary>
		/// Get the number of available tickets for the selected list of brands from the cache if found
		/// Otherwise from the db after caching it.
		/// </summary>
		Task<int> GetAvailableTickets(Guid advocateId, TicketLevel level);

		/// <summary>
		/// Add a ticket to the cached list for this brand
		/// </summary>
		void AddTicket(Guid ticketId, Guid brandId, TicketLevel level);

		/// <summary>
		/// Mark ticket as touched by advocate
		/// </summary>
		void TouchTicket(Guid ticketId, Guid advocateId);

		/// <summary>
		/// Remove the ticket completely from the cache list
		/// Mostly because it is closed/escalated permenantly.
		/// </summary>
		void RemoveTicket(Guid ticketId, Guid brandId);

		/// <summary>
		/// Gets the cached public profile.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <returns>The cached brand.</returns>
		Task<Brand> GetAsync(Guid brandId);


		/// <summary>
		/// Get the list of active brands for an advocate (from cache/db).
		/// </summary>
		Task<IList<Guid>> GetActiveBrandsIds(Guid advocateId);

		/// <summary>
		/// Remove ticket as touched by advocate
		/// </summary>
		void UntouchTicket(Guid ticketId, Guid advocateId);
	}
}

