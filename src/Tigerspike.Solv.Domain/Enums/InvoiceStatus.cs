namespace Tigerspike.Solv.Domain.Enums
{
	public enum InvoiceStatus
	{
		/// <summary>
		/// No payments were made against this invoice yet.
		/// </summary>
		Unpaid,

		/// <summary>
		/// Payments were made but doesn't fully cover the full amount.
		/// This indicates that some line item payments were failed, and need to be retried (or ignored).
		/// </summary>
		PartiallyPaid,

		/// <summary>
		/// Payments were made against the full amount.
		/// </summary>
		FullyPaid
	}
}
