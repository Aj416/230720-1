using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Services.Fraud.Domain;

namespace Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces
{
	/// <summary>
	/// Ticket repository to connect with dynamo db.
	/// </summary>
	public interface ITicketRepository
	{
		/// <summary>
		/// Adds or updates the item in the store.
		/// </summary>
		/// <param name="ticket">The ticket to add.</param>
		/// <returns>The created or updated ticket.</returns>
		Ticket AddOrUpdateTicket(Ticket ticket);

		/// <summary>
		/// Deletes the item based on the ticket id.
		/// </summary>
		/// <param name="ticketId">The ticket id.</param>
		/// <returns></returns>
		void DeleteTicket(Guid ticketId);

		/// <summary>
		/// Gets ticket for specified id.
		/// </summary>
		/// <param name="id">The ticket id.</param>
		/// <returns>Ticket for specified id.</returns>
		Ticket GetTicket(string id);

		/// <summary>
		/// Returns ids of tickets with valid fraud detection.
		/// </summary>
		/// <returns>Ids of tickets with valid fraud detection.</returns>
		Task<IEnumerable<Guid>> GetFraudTicketIds();

		/// <summary>
		/// Updates the list of tickets.
		/// </summary>
		/// <param name="tickets">The tickets to be updated.</param>
		/// <returns></returns>
		void BulkUpdateTicket(IEnumerable<Ticket> tickets);

		/// <summary>
		/// Get fraud ids.
		/// </summary>
		/// <returns>List of fraud ids.</returns>
		Task<List<Guid>> GetFraudIds();

		/// <summary>
		/// Get Customer Tickets
		/// </summary>
		/// <returns>List of customer tickets.</returns>
		Task<List<Ticket>> GetCustomerTicket(string customerId);
	}
}