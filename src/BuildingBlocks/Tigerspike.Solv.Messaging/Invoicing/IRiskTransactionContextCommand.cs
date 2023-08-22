using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IRiskTransactionContextCommand
	{
		/// <summary>
		/// Generated unique identifier used to track set risk transaction context
		/// </summary>
		Guid TrackingId { get; set; }

		/// <summary>
		/// Sender's account id
		/// </summary>
		Guid SenderAccountId { get; set; }

		/// <summary>
		/// Date of creation sender's account on the platform
		/// </summary>
		DateTime SenderCreatedDate { get; set; }

		/// <summary>
		/// Receiver's account id
		/// </summary>
		Guid ReceiverAccountId { get; set; }

		/// <summary>
		/// Date of creation receiver's account on the platform
		/// </summary>
		DateTime ReceiverCreatedDate { get; set; }

		/// <summary>
		/// The paypal account id of the receiver.
		/// </summary>
		string ReceiverPayPalAccountId { get; set; }

		/// <summary>
		/// Receiver's email
		/// </summary>
		string ReceiverEmail { get; set; }

		/// <summary>
		/// Receiver's country code
		/// </summary>
		string ReceiverAddressCountryCode { get; set; }

		/// <summary>
		/// Receiver's CSAT score
		/// </summary>
		decimal? ReceiverPopularityScore { get; set; }

		/// <summary>
		/// Date of first interaction (transaction) between sender and receiver on the platform
		/// </summary>
		DateTime FirstInteractionDate { get; set; }

		/// <summary>
		/// Total number of transactions that sender has had to date on the platform
		/// </summary>
		int TxnCountTotal { get; set; }
	}
}
