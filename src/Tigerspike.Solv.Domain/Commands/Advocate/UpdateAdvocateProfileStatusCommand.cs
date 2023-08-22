using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class UpdateAdvocateProfileStatusCommand : Command
	{
		/// <summary>
		/// Advocate Identifier.
		/// </summary>
		public Guid AdvocateId { get; }

		/// <summary>
		/// ProfileQuestionId Identifier.
		/// </summary>
		public Guid ProfileQuestionId { get; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="advocateId">Advocate Identifier.</param>
		/// <param name="profileQuestionId">ProfileQuestionId Identifier.</param>
		public UpdateAdvocateProfileStatusCommand(Guid advocateId, Guid profileQuestionId) => (AdvocateId, ProfileQuestionId) = (advocateId, profileQuestionId);

		public override bool IsValid() => AdvocateId != Guid.Empty && ProfileQuestionId != Guid.Empty;
	}
}
