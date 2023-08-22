using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Infra.Data.Models
{
	/// <summary>
	/// Ticket model for data that used for transport handling
	/// </summary>
	public class TicketTransportModel
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The ticket number.
		/// </summary>
		public long Number { get; set; }

		/// <summary>
		/// The customer email
		/// </summary>
		public string CustomerEmail { get; set; }

		/// <summary>
		/// The customer first name
		/// </summary>
		public string CustomerFirstName { get; set; }

		/// <summary>
		/// The advocate first name
		/// </summary>
		public string AdvocateFirstName { get; set; }

		/// <summary>
		/// Conversation transport used for the ticket
		/// </summary>
		public TicketTransportType TransportType { get; set; }

		/// <summary>
		/// Ticket brand's name
		/// </summary>
		public string BrandName { get; set; }

		/// <summary>
		/// Ticket brand's logo url
		/// </summary>
		public string BrandLogoUrl { get; set; }

		/// <summary>
		/// Ticket question
		/// </summary>
		public string Question { get; set; }

		/// <summary>
		/// The last response on the ticket
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Url to chat interface
		/// </summary>
		/// <value></value>
		public string ChatUrl { get; set; }

		/// <summary>
		/// Url to close ticket interface
		/// </summary>
		/// <value></value>
		public string RateUrl { get; set; }

		/// <summary>
		/// Determines if end chat feature is applicable on the brand.
		/// </summary>
		public bool EndChatEnabled { get; set; }

		/// <summary>
		/// Ticket culture
		/// </summary>
		public string Culture { get; set;}

		/// <summary>
		/// Closing time frame of the ticket
		/// </summary>
		public int ClosingTime { get; set; }
	}
}