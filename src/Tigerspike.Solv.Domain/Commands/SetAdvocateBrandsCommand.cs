using System;
using System.Collections.Generic;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	/// <summary>
	/// Set brands assigned to an advocate
	/// </summary>
	public class SetAdvocateBrandsCommand : Command<Unit>
	{
		public Guid AdvocateId { get; }

		public IEnumerable<Guid> BrandIds { get; }

		public bool Authorised { get; }

		public SetAdvocateBrandsCommand(Guid advocateId, IEnumerable<Guid> brandIds, bool authorised)
		{
			AdvocateId = advocateId;
			BrandIds = brandIds;
			Authorised = authorised;
		}

		public override bool IsValid() => AdvocateId != Guid.Empty;
	}
}