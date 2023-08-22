using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tigerspike.Solv.Core.Domain;
using Tigerspike.Solv.Core.Services;

namespace Tigerspike.Solv.Core.Behaviours
{
	/// <summary>
	/// Behavior for setting timestamp on created entities
	/// </summary>
	public class CreatedDateBehaviour : IContextBehaviour
	{
		private readonly ITimestampService _timestampService;

		public CreatedDateBehaviour(ITimestampService timestampService) => _timestampService = timestampService;

		/// <summary>
		/// Sets timestamp in CreatedDate field for created entities
		/// </summary>
		/// <param name="entries">All entries in the current context</param>
		public void Apply(IEnumerable<EntityEntry> entries)
		{
			entries
				.Where(x => x.State == EntityState.Added)
				.Select(x => x.Entity)
				.OfType<ICreatedDate>()
				.ToList()
				.ForEach(x => x.CreatedDate = _timestampService.GetUtcTimestamp());
		}
	}
}
