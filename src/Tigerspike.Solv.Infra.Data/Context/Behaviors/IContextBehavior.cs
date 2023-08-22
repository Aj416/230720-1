using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Tigerspike.Solv.Infra.Data.Context.Behaviors
{
	public interface IContextBehavior
	{
		/// <summary>
		/// Applies desired behavior on all eligble entities
		/// </summary>
		/// <param name="entries">All entries that are in current context</param>
		void Apply(IEnumerable<EntityEntry> entries);
	}

}
