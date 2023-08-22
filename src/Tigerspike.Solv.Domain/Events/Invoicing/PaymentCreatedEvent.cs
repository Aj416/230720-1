using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.Invoicing
{
	public class PaymentCreatedEvent : Event
	{
		/// <summary>
		/// The id of the payment
		/// </summary>
		public Guid PaymentId { get; set; }

		/// <summary>
		/// If the payment was made for an advocate.
		/// </summary>
		public Guid? AdvocateId { get; set; }

		/// <summary>
		/// If the payment was taken from a brand.
		/// </summary>
		public Guid? BrandId { get; set; }

		public PaymentCreatedEvent(Guid id, Guid? advocateId, Guid? brandId) => (PaymentId, AdvocateId, BrandId) = (id, advocateId, brandId);
	}
}