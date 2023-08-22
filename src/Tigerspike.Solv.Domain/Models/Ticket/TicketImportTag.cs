using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Represent a relation between a ticket and a tag.
	/// </summary>
	public class TicketImportTag
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketImportId { get; private set; }

		/// <summary>
		/// The tag identifier.
		/// </summary>
		public Guid TagId { get; private set; }

		/// <summary>
		/// Related tag
		/// </summary>
		public Tag Tag { get; private set; }

		/// <summary>
		/// Constructor to please EF.
		/// </summary>
		private TicketImportTag() { }

		public TicketImportTag(Guid ticketImportId, Guid tagId) =>
			(TicketImportId, TagId) = (ticketImportId, tagId);
	}
}