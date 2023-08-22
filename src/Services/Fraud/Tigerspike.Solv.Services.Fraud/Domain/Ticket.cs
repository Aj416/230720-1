using System;
using System.Collections.Generic;
using System.Globalization;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Services.Fraud.Enum;

namespace Tigerspike.Solv.Services.Fraud.Domain
{
	[References(typeof(TicketFraudStatusGlobalIndex))]
	public class Ticket
	{
		private const string DateTimeIsoFormat = "yyyy-MM-ddTHH:mm:ss'Z'";

		/// <summary>
		/// Gets or sets the ticket Id.
		/// </summary>
		[HashKey]
		public string TicketId { get; set; }

		/// <summary>
		/// Gets or sets the created date
		/// </summary>
		[RangeKey]
		public string CreatedDate { get; set; } // Sort key

		/// <summary>
		/// Gets or sets the customer id.
		/// </summary>
		public string CustomerId { get; set; } // sort key

		/// <summary>
		/// Gets or sets the current solver assigned to the ticket.
		/// </summary>
		public string AssignedTo { get; set; }

		/// <summary>
		/// Gets or sets the brand id of the ticket.
		/// </summary>
		public string BrandId { get; set; }

		/// <summary>
		/// Gets or sets the brand name of the ticket.
		/// </summary>
		public string BrandName { get; set; }

		/// <summary>
		/// Gets or sets the level of the ticket.
		/// </summary>
		public TicketLevel Level { get; set; }

		/// <summary>
		/// Gets or sets the status of the ticket.
		/// </summary>
		public TicketStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the last fraud result.
		/// </summary>
		public FraudStatus FraudStatus { get; private set; }

		/// <summary>
		/// The customer details.
		/// </summary>
		public Customer CustomerDetail { get; set; }

		/// <summary>
		/// The key value pair for ticket metadata.
		/// </summary>
		public IDictionary<string, string> Metadata { get; set; }

		/// <summary>
		/// Gets or sets question asked by customer.
		/// </summary>
		public string Question { get; set; }

		/// <summary>
		/// Parameterised construtor
		/// </summary>
		/// <param name="ticketId">Ticket Identifier.</param>
		/// <param name="customerId">Customer Identifier.</param>
		/// <param name="brandId">Brand Identifier.</param>
		/// <param name="brandName">Brand name.</param>
		/// <param name="level">Ticket level.</param>
		/// <param name="status">Ticket status.</param>
		/// <param name="advocateName">Advocate name.</param>
		/// <param name="metadata">Customer metadat.</param>
		/// <param name="customerDetail">Customer detail.</param>
		/// <param name="question">Customer question.</param>
		public Ticket(Guid ticketId, Guid? customerId, Guid brandId, string brandName, int level,
			int status, string advocateName, IDictionary<string, string> metadata, Customer customerDetail, string question)
		{
			TicketId = ticketId.ToString();
			CreatedDate = DateTime.UtcNow.ToString(DateTimeIsoFormat, CultureInfo.InvariantCulture);
			BrandName = brandName;
			CustomerId = customerId.ToString() ?? null;
			BrandId = brandId.ToString();
			Level = (TicketLevel)level;
			Status = (TicketStatus)status;
			AssignedTo = advocateName;
			Metadata = metadata;
			CustomerDetail = customerDetail;
			Question = question;
		}

		public void UpdateTicket(string advocateName, int level, int status)
		{
			AssignedTo = advocateName;
			Level = (TicketLevel)level;
			Status = (TicketStatus)status;
		}

		public void SetFraudResult(FraudStatus status) => FraudStatus = status;
	}
}