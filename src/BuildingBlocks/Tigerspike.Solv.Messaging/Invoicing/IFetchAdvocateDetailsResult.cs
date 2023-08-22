using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchAdvocateDetailsResult : IResult
	{
		/// <summary>
		/// Advocate Created date.
		/// </summary>
		DateTime CreatedDate { get; set; }

		/// <summary>
		/// Advocate email.
		/// </summary>
		string Email { get; set; }

		/// <summary>
		/// Advocate brand csat.
		/// </summary>
		decimal Csat { get; set; }

		/// <summary>
		/// Payment account identifier.
		/// </summary>
		string PaymentAccountId { get; set; }
	}
}
