using System;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Represent a failure reason for the import of particular ticket
	/// </summary>
	public class TicketImportFailure
	{
		/// <summary>
		/// The entity identifier
		/// </summary>
		/// <value></value>
		public Guid Id { get; }

		/// <summary>
		/// The ticket import identifier.
		/// </summary>
		public Guid TicketImportId { get; }

		/// <summary>
		/// The raw message input (as in file)
		/// </summary>
		public string RawInput { get; }

		/// <summary>
		/// The import failure reason
		/// </summary>
		public string FailureReason { get; }

		public TicketImportFailure(Guid ticketImportId, string rawInput, string failureReason)
		{
			TicketImportId = ticketImportId;
			RawInput = rawInput;
			FailureReason = failureReason;
		}
	}
}