using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchBrandBillingDetailResult : IResult
	{
		/// <summary>
		/// Gets or sets billing details Id.
		/// </summary>
		Guid BillingDetailsId { get; set; }

		/// <summary>
		/// Gets or sets VatRate.
		/// </summary>
		decimal? VatRate { get; set; }
	}
}
