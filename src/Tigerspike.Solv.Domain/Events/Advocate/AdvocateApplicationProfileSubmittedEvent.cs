using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateApplicationProfileSubmittedEvent : Event
	{
		/// <summary>
		/// Advocate Identifier.
		/// </summary>
		public Guid AdvocateId { get; private set; }

		/// <summary>
		/// ProfileQuestionId Identifier.
		/// </summary>
		public Guid ProfileQuestionId { get; private set; }

		/// <summary>
		/// AdvocateApplicationProfileSubmittedEvent Constructor.
		/// </summary>
		/// <param name="advocateId">Advocate Identifier.</param>
		/// <param name="profileQuestionId">ProfileQuestionId Identifier.</param>
		public AdvocateApplicationProfileSubmittedEvent(Guid advocateId, Guid profileQuestionId) => (AdvocateId, ProfileQuestionId) = (advocateId, profileQuestionId);
	}
}
