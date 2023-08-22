using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	/// <summary>
	/// Indicates that an advocate has just been created
	/// and we need to assign the brands decided in his application
	/// to him/her (if any)
	/// </summary>
	public class AssignBrandsToNewAdvocateCommand : Command<Unit>
	{
		public Guid AdvocateId { get; }

		public AssignBrandsToNewAdvocateCommand(Guid advocateId) => AdvocateId = advocateId;

		public override bool IsValid() => AdvocateId != Guid.Empty;
	}
}
