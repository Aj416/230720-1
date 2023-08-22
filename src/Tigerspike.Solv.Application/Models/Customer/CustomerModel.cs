using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Customer
{
	public class CustomerModel
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public IEnumerable<CustomerTicketModel> Tickets { get; set; }
	}

	public class CustomerTicketModel
	{
		/// <summary>
		/// The Id of the ticket.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The Question of the ticket.
		/// </summary>
		public string Question { get; set; }

		/// <summary>
		/// The status of the ticket.
		/// </summary>
		public TicketStatusEnum Status { get; set; }

		/// <summary>
		/// The creation date of the ticket.
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The last modification date of the ticket.
		/// </summary>
		public DateTime? ModifiedDate { get; set; }

		/// <summary>
		/// The ChatUrl of the ticket.
		/// </summary>
		public string ChatUrl { get; set; }

		/// <summary>
		/// Ticket metadata
		/// </summary>
		public IDictionary<string, string> Metadata { get; set; }
	}
}