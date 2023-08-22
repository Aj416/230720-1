using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Services.Invoicing.Application.IntegrationEvents
{
	public class BillingDetailsCreatedEvent : Event
	{
		/// <summary>
		/// Brand identifier.
		/// </summary>
		public Guid BrandId { get; private set; }

		/// <summary>
		/// BillingDetails Identifier. 
		/// </summary>
		public Guid BillingDetailsId { get; private set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="billingDetailsId">BillingDetails Id.</param>
		/// <param name="brandId">Brand Id.</param>
		public BillingDetailsCreatedEvent(Guid billingDetailsId, Guid brandId)
		{
			BillingDetailsId = billingDetailsId;
			BrandId = brandId;
		}
	}
}
