using System;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Ticket specific category data
	/// </summary>
	public class TicketCategory : ICreatedDate
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; private set; }

		/// <summary>
		/// The category identifier.
		/// </summary>
		public Guid CategoryId { get; private set; }

		/// <summary>
		/// Related category
		/// </summary>
		public Category Category { get; private set; }

		/// <summary>
		/// Get or sets the user id who set the tag.
		/// </summary>
		public Guid? UserId { get; set; }

		/// <summary>
		/// Gets or sets tag created date.
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Default TicketCategory constructor
		/// </summary>
		public TicketCategory() { }

		public TicketCategory(Guid ticketId, Guid categoryId, Guid? userId)
		{
			TicketId = ticketId;
			CategoryId = categoryId;
			UserId = userId;
		}
	}
}
