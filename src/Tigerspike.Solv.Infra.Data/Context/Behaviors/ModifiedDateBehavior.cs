using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Context.Behaviors
{

	/// <summary>
	/// Behavior for setting timestamp on modified/created entities
	/// </summary>
	public class ModifiedDateBehavior : IContextBehavior
	{
		private readonly ITimestampService _timestampService;

		public ModifiedDateBehavior(ITimestampService timestampService)
		{
			_timestampService = timestampService;
		}

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
