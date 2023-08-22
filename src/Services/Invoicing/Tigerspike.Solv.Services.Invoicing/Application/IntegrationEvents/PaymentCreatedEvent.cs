using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Services.Invoicing.Application.IntegrationEvents
{
	public class PaymentCreatedEvent : Event
	{
		/// <summary>
		/// If the payment was made for an advocate.
		/// </summary>
		public Guid? AdvocateId { get; set; }

		public PaymentCreatedEvent(Guid? advocateId) => AdvocateId = advocateId;
	}
}
