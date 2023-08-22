using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tigerspike.Solv.Core.Domain;
using Tigerspike.Solv.Core.Services;

namespace Tigerspike.Solv.Core.Behaviours
{
	public class ModifiedDateBehaviour : IContextBehaviour
	{
		private readonly ITimestampService _timestampService;

		public ModifiedDateBehaviour(ITimestampService timestampService) => _timestampService = timestampService;

		/// <summary>
		/// Sets timestamp in ModifiedDate field for modified/created entities
		/// </summary>
		/// <param name="entries">All entries in the current context</param>
		public void Apply(IEnumerable<EntityEntry> entries)
		{
			entries
				.Where(x => x.State == EntityState.Added || x.State == EntityState.Modified)
				.Select(x => x.Entity)
				.OfType<IModifiedDate>()
				.ToList()
				.ForEach(x => x.ModifiedDate = _timestampService.GetUtcTimestamp());
		}
	}
}
