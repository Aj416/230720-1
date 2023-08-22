using System;
using Tigerspike.Solv.Services.Invoicing.Enums;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class AdvocateInvoiceModel
	{
		/// <summary>
		/// The system identifier of the invoice
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// A friendly generated identifier of this invoice to be displayed.
		/// </summary>
		public string ReferenceNumber { get; set; }

		/// <summary>
		/// Issue date of the invoice
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Total amount on the invoice
		/// </summary>
		public decimal Total { get; set; }

		/// <summary>
		/// The advocate identifier.
		/// </summary>
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// Advocate on the invoice
		/// </summary>
		public string AdvocateFullName { get; set; }

		public AdvocateStatus AdvocateStatus { get; set; }

		/// <summary>
		/// The invoice status
		/// </summary>
		public AdvocateInvoiceStatus Status { get; set; }

		/// <summary>
		/// The date of the payment failure.
		/// </summary>
		public DateTime? PaymentFailureDate { get; set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="advocateFullName">Advocate full name.</param>
		/// <param name="advocateStatus">Advcate status.</param>
		public AdvocateInvoiceModel(string advocateFullName, int advocateStatus, Guid id, string referenceNumber, DateTime createdDate, decimal total, Guid advocateId, AdvocateInvoiceStatus status, DateTime? paymentFailureDate)
		{
			AdvocateFullName = advocateFullName;
			AdvocateStatus = (AdvocateStatus)advocateStatus;
			Id = id;
			ReferenceNumber = referenceNumber;
			CreatedDate = createdDate;
			Total = total;
			AdvocateId = advocateId;
			Status = status;
			PaymentFailureDate = paymentFailureDate;
		}

		public AdvocateInvoiceModel()
		{
			
		}
	}
}
