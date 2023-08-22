using System;
using System.Collections.Generic;
using System.Linq;
using Tigerspike.Solv.Core.Domain;
using Tigerspike.Solv.Core.Exceptions;
using Tigerspike.Solv.Services.Invoicing.Enums;

namespace Tigerspike.Solv.Services.Invoicing.Domain
{
	public class AdvocateInvoice : ICreatedDate, IModifiedDate
	{
		/// <summary>
		/// Constructor to please EF.
		/// </summary>
		private AdvocateInvoice() { }

		public AdvocateInvoice(Guid invoicingCycleId, Guid platformBillingDetailsId, Guid advocateId, int sequence, IEnumerable<AdvocateInvoiceLineItem> lines)
		{
			InvoicingCycleId = invoicingCycleId;
			PlatformBillingDetailsId = platformBillingDetailsId;
			AdvocateId = advocateId;
			LineItems = lines.ToList();
			Total = LineItems.Sum(x => x.Amount);
			Sequence = sequence;
			ReferenceNumber = $"INV{sequence.ToString("D4")}";
		}

		public Guid Id { get; set; }

		/// <summary>
		/// The billing details of platform
		/// </summary>
		public Guid PlatformBillingDetailsId { get; set; }

		/// <summary>
		/// The billing details of platform
		/// </summary>
		public BillingDetails PlatformBillingDetails { get; set; }

		public DateTime CreatedDate { get; set; }

		public DateTime ModifiedDate { get; set; }

		/// <summary>
		/// The advocate of this invoice.
		/// </summary>
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// The invoicing cycle that this invoice falls into.
		/// </summary>
		public Guid InvoicingCycleId { get; set; }

		/// <summary>
		/// The invoicing cycle that this invoice falls into.
		/// </summary>
		public InvoicingCycle InvoicingCycle { get; set; }

		/// <summary>
		/// Total amount to be paid to the advocate according to this invoice.
		/// </summary>
		public decimal Total { get; set; }

		/// <summary>
		/// Total amount to be paid to the advocate according to this invoice.
		/// </summary>
		public decimal AmountPaid { get; set; }

		/// <summary>
		/// A friendly generated identifier of this invoice to be displayed.
		/// </summary>
		public string ReferenceNumber { get; set; }

		/// <summary>
		/// Seqeunce number of the invoice, used to generated reference number
		/// </summary>
		/// <value></value>
		public int? Sequence { get; set; }

		/// <summary>
		/// Details of the invoice. each line represents a brand specific information.
		/// </summary>
		public List<AdvocateInvoiceLineItem> LineItems { get; set; }

		/// <summary>
		/// The status of the invoice according to payments made.
		/// </summary>
		public InvoiceStatus Status { get; set; }

		/// <summary>
		/// The date of the payment failure.
		/// </summary>
		public DateTime? PaymentFailureDate { get; set; }

		/// <summary>
		/// Set the date of the payment against this invoice that has failed.
		/// </summary>
		public void SetFailureDate(DateTime date) => PaymentFailureDate = date;

		/// <summary>
		/// Set a new paid payment was made against this invoice.
		/// </summary>
		/// <param name="amount">The amount of the new payment</param>
		public void SetPaidAmount(decimal amount)
		{
			AmountPaid += amount;
			if (AmountPaid > Total || amount <= 0)
			{
				throw new DomainException("Paid amount is invalid, should be 0 < Amount <= Total");
			}
			Status = AmountPaid == Total ? InvoiceStatus.FullyPaid : InvoiceStatus.PartiallyPaid;
		}
	}
}
