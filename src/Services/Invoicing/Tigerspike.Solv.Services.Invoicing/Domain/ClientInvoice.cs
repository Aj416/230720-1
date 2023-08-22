using System;
using Tigerspike.Solv.Core.Domain;
using Tigerspike.Solv.Core.Exceptions;
using Tigerspike.Solv.Services.Invoicing.Enums;

namespace Tigerspike.Solv.Services.Invoicing.Domain
{
	public class ClientInvoice : ICreatedDate, IModifiedDate
	{
		/// <summary>
		/// Constructor to please EF.
		/// </summary>
		private ClientInvoice() { }

		public ClientInvoice(Guid brandId, Guid invoicingCycleId, decimal price, decimal fee, decimal subtotal, decimal? vatRate, decimal? vatAmount, decimal invoiceTotal, decimal paymentTotal, int ticketsCount, int sequence, Guid platformBillingDetailsId, Guid brandBillingDetailsId)
		{
			BrandId = brandId;
			InvoicingCycleId = invoicingCycleId;
			Price = price;
			Fee = fee;
			Subtotal = subtotal;
			VatRate = vatRate;
			VatAmount = vatAmount;
			InvoiceTotal = invoiceTotal;
			PaymentTotal = paymentTotal;
			TicketsCount = ticketsCount;
			Sequence = sequence;
			ReferenceNumber = $"INV{sequence.ToString("D4")}";

			PlatformBillingDetailsId = platformBillingDetailsId;
			BrandBillingDetailsId = brandBillingDetailsId;
		}

		public Guid Id { get; private set; }

		/// <summary>
		/// Seqeunce number of the invoice, used to generated reference number
		/// </summary>
		/// <value></value>
		public int? Sequence { get; set; }

		/// <summary>
		/// The billing details of platform
		/// </summary>
		public Guid PlatformBillingDetailsId { get; set; }

		/// <summary>
		/// The billing details of platform
		/// </summary>
		public BillingDetails PlatformBillingDetails { get; set; }

		/// <summary>
		/// The brand billing details
		/// </summary>
		public Guid BrandBillingDetailsId { get; set; }

		/// <summary>
		/// The brand billing details
		/// </summary>
		public BillingDetails BrandBillingDetails { get; set; }

		/// <summary>
		/// The brand that this invoice belongs to.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// A friendly generated identifier of this invoice to be displayed.
		/// </summary>
		public string ReferenceNumber { get; set; }

		/// <summary>
		/// The invoicing cycle that this invoice falls into.
		/// </summary>
		public Guid InvoicingCycleId { get; set; }

		/// <summary>
		/// The invoicing cycle that this invoice falls into.
		/// </summary>
		public InvoicingCycle InvoicingCycle { get; set; }

		/// <summary>
		/// The creation date of this invoice.
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The modification date of this invoice.
		/// </summary>
		public DateTime ModifiedDate { get; set; }

		/// <summary>
		/// Total Solv fees of all tickets in this invoice (that the Solv will receive).
		/// </summary>
		public decimal Fee { get; set; }

		/// <summary>
		/// Total price of all tickets in this invoice (that the advocate will receive).
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// Number of tickets invoiced
		/// </summary>
		public int TicketsCount { get; set; }

		/// <summary>
		/// Subtotal amount (excluding taxes)
		/// </summary>
		public decimal Subtotal { get; set; }

		/// <summary>
		/// VAT rate
		/// </summary>
		public decimal? VatRate { get; set; }

		/// <summary>
		/// VAT amount
		/// </summary>
		public decimal? VatAmount { get; set; }

		/// <summary>
		/// Total amount on payment advice (including taxes)
		/// </summary>
		public decimal PaymentTotal { get; set; }

		/// <summary>
		/// Total amount on invoice (after taxes)
		/// </summary>
		public decimal InvoiceTotal { get; set; }

		/// <summary>
		/// Total amount to be paid to the advocate according to this invoice.
		/// </summary>
		public decimal AmountPaid { get; set; }

		/// <summary>
		/// The status of the invoice according to payments made.
		/// </summary>
		public InvoiceStatus Status { get; set; }

		/// <summary>
		/// The date of the payment failure.
		/// </summary>
		public DateTime? PaymentFailureDate { get; set; }

		/// <summary>
		/// Set a new paid payment was made against this invoice.
		/// </summary>
		/// <param name="amount">The amount of the new payment</param>
		public void SetPaidAmount(decimal amount)
		{
			AmountPaid += amount;
			if (AmountPaid > InvoiceTotal || amount <= 0)
			{
				throw new DomainException("Paid amount is invalid, should be 0 < Amount <= Total");
			}
			Status = AmountPaid == InvoiceTotal ? InvoiceStatus.FullyPaid : InvoiceStatus.PartiallyPaid;
		}

		/// <summary>
		/// Set the date of the payment against this invoice that has failed.
		/// </summary>
		public void SetFailureDate(DateTime date) => PaymentFailureDate = date;
	}
}
