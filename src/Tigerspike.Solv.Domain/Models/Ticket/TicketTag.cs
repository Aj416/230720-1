using System;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Represent a relation between a ticket and a tag.
	/// </summary>
	public class TicketTag
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; private set; }

		/// <summary>
		/// The tag identifier.
		/// </summary>
		public Guid TagId { get; private set; }

		/// <summary>
		/// Related tag
		/// </summary>
		public Tag Tag { get; private set; }

		/// <summary>
		/// Level of the tag
		/// </summary>
		public TicketLevel Level { get; set; }

		/// <summary>
		/// Get or sets the user id who set the tag.
		/// </summary>
		public Guid? UserId { get; set; }

		/// <summary>
		/// Gets or sets tag created date.
		/// </summary>
		public DateTime? CreatedDate { get; set; }

		/// <summary>
		/// Constructor to please EF.
		/// </summary>
		private TicketTag() { }

		public TicketTag(Guid ticketId, Guid tagId, TicketLevel level, Guid? userId, DateTime? createdDate) =>
			(TicketId, TagId, Level, UserId, CreatedDate) = (ticketId, tagId, level, userId, createdDate);
	}
}