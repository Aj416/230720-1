using System;
using System.Collections.Generic;
using Tigerspike.Solv.Services.Fraud.Domain;

namespace Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces
{
	/// <summary>
	/// Ticket detection repository to connect with dynamo db.
	/// </summary>
	public interface ITicketDetectionRepository
	{
		/// <summary>
		/// Gets the ticketDetection list based on the specified ticket id.
		/// </summary>
		/// <param name="ticketId">The ticket id.</param>
		/// <returns>The list of detection for the ticket id.</returns>
		List<TicketDetection> GetList(Guid ticketId);

		/// <summary>
		/// Adds or updates the item in the store.
		/// </summary>
		/// <param name="ticketDetection">The ticketDetection to add.</param>
		/// <returns>The created or updated ticketDetection.</returns>
		TicketDetection AddOrUpdateDetection(TicketDetection ticketDetection);

		/// <summary>
		/// Deletes the item based on the detectionId id.
		/// </summary>
		/// <param name="detectionId">The detectionId id.</param>
		/// <returns></returns>
		void DeleteDetection(string detectionId);

		/// <summary>
		/// Returns ticket detection for specified ticket id.
		/// </summary>
		/// <param name="id">The ticket id.</param>
		/// <returns>Ticket detection for specified ticket id</returns>
		TicketDetection GetTicketDetection(string id);
	}
}