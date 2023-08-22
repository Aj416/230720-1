using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Application.Interfaces
{
	/// <summary>
	/// The metadata service for the brand
	/// </summary>
	public interface IBrandMetadataService
	{
		/// <summary>
		/// Method to assign the level of the ticket
		/// </summary>
		/// <param name="ticket"></param>
		/// <param name="metadata"></param>
		/// <returns>The assigning ticket level</returns>
		Task RouteToLevel(Ticket ticket, IEnumerable<TicketMetadataItem> metadata);

		/// <summary>
		/// Method to filter the metadata based on the metadata access level of the ticket brand
		/// </summary>
		/// <param name="ticket"></param>
		/// <param name="level">Access Level</param>
		/// <returns></returns>
		Task FilterMetadata(Ticket ticket, AccessLevel level);

		/// <summary>
		/// Method to filter the metadata based on the metadata access level of each ticket's brand
		/// </summary>
		/// <param name="tickets"></param>
		/// <returns></returns>
		Task FilterMetadata(IEnumerable<Ticket> tickets);
	}
}