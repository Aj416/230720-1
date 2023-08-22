using System;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class RiskTransactionContext : IRiskTransactionContextCommand
	{
		/// <summary>
		/// Generated unique identifier used to track set risk transaction context
		/// </summary>
		public Guid TrackingId { get; set; }

		/// <summary>
		/// Sender's account id
		/// </summary>
		public Guid SenderAccountId { get; set; }

		/// <summary>
		/// Date of creation sender's account on the platform
		/// </summary>
		public DateTime SenderCreatedDate { get; set; }

		/// <summary>
		/// Receiver's account id
		/// </summary>
		public Guid ReceiverAccountId { get; set; }

		/// <summary>
		/// Date of creation receiver's account on the platform
		/// </summary>
		public DateTime ReceiverCreatedDate { get; set; }

		/// <summary>
		/// The paypal account id of the receiver.
		/// </summary>
		public string ReceiverPayPalAccountId { get; set; }

		/// <summary>
		/// Receiver's email
		/// </summary>
		public string ReceiverEmail { get; set; }

		/// <summary>
		/// Receiver's country code
		/// </summary>
		public string ReceiverAddressCountryCode { get; set; }

		/// <summary>
		/// Receiver's CSAT score
		/// </summary>
		public decimal? ReceiverPopularityScore { get; set; }

		/// <summary>
		/// Date of first interaction (transaction) between sender and receiver on the platform
		/// </summary>
		public DateTime FirstInteractionDate { get; set; }

		/// <summary>
		/// Total number of transactions that sender has had to date on the platform
		/// </summary>
		public int TxnCountTotal { get; set; }

	}
}
