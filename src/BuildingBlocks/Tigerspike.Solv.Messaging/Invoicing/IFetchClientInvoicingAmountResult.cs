namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchClientInvoicingAmountResult : IResult
	{
		/// <summary>
		/// Gets or sets PriceTotal.
		/// </summary>
		decimal PriceTotal { get; set; }

		/// <summary>
		/// Gets or sets FeeTotal.
		/// </summary>
		decimal FeeTotal { get; set; }

		/// <summary>
		/// Gets or sets TicketCount.
		/// </summary>
		int TicketCount { get; set; }
	}
}
