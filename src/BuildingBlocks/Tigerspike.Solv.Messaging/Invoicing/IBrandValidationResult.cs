using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IBrandValidationResult : IResult
	{
		/// <summary>
		/// Gets or sets payment route name.
		/// </summary>
		string PaymentRouteName { get; set; }

		/// <summary>
		/// Brand name.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// PaymentAccount Identifier.
		/// </summary>
		string PaymentAccountId { get; set; }

		/// <summary>
		/// Billing Agreement identifier.
		/// </summary>
		string BillingAgreementId { get; set; }

		/// <summary>
		/// Brand creation date.
		/// </summary>
		DateTime CreatedDate { get; set; }
	}
}
