namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Invoice list sorting options
	/// </summary>
	public enum InvoiceSortBy
	{
		/// Issue date of invoice
		CreatedDate,
		/// Tickets count accounted on invoice
		TicketsCount,
		/// Total amount on invoice
		InvoiceTotal,
		/// Subtotal amount of an invoice
		Subtotal,
		/// Reference number on invoice
		ReferenceNumber
	}
}