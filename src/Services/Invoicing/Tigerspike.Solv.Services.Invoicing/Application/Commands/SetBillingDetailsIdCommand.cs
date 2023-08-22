using System;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Application.Commands
{
	public class SetBillingDetailsIdCommand : ISetBillingDetailsIdCommand
	{
		/// <summary>
		/// Brand Identifier.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// BillingDetails Identifier.
		/// </summary>
		public Guid BillingDetailsId { get; set; }

		/// <summary>
		/// Paramerised constructor.
		/// </summary>
		/// <param name="brandId">Brand id.</param>
		/// <param name="billingDetailsId">BillingDetails Id.</param>
		public SetBillingDetailsIdCommand(Guid brandId, Guid billingDetailsId)
		{
			BrandId = brandId;
			BillingDetailsId = billingDetailsId;
		}
	}
}
