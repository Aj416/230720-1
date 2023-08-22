using System;
using Tigerspike.Solv.Core.Domain;

namespace Tigerspike.Solv.Services.Invoicing.Domain
{
	/// <summary>
	/// A payment is an actual money transfer from a brand to an advocate.
	/// </summary>
	public class Payment : ICreatedDate
	{
		public Guid Id { get; set; }

		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The amount paid by this payment.
		/// </summary>
		public decimal Amount { get; set; }

		/// <summary>
		/// A friendly generated identifier of this payment to be displayed.
		/// </summary>
		public string ReferenceNumber { get; set; }

		/// <summary>
		/// The client invoice id which based on, this payment was created.
		/// </summary>
		public Guid? ClientInvoiceId { get; set; }

		/// <summary>
		/// The advocate invoice line item that this payment was raised against.
		/// </summary>
		public AdvocateInvoiceLineItem AdvocateInvoiceLineItem { get; set; }

		/// <summary>
		/// The advocate invoice line item that this payment was raised against.
		/// </summary>
		public Guid? AdvocateInvoiceLineItemId { get; set; }

	}
}
