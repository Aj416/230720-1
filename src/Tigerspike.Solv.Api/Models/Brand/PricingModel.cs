namespace Tigerspike.Solv.Api.Models.Brand
{
	/// <summary>
	/// Pricing information about tickets from a brand
	/// </summary>
    public class PricingModel
    {
		/// <summary>
		/// Price for new tickets
		/// </summary>
		/// <value></value>
        public decimal TicketPrice { get; set; }

		/// <summary>
		/// Absolute value of fee charged by Solv
		/// </summary>
		public decimal Fee { get; set; }

		/// <summary>
		/// Percentage value of fee charged by Solv
		/// </summary>
		public decimal FeePercentage { get; set; }

		/// <summary>
		/// Absolute value of total ticket price (Price + Fee)
		/// </summary>
		public decimal TotalTicketPrice => TicketPrice + Fee;
    }
}