using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Context.Behaviors
{

	/// <summary>
	/// Behavior for setting timestamp on created entities
	/// </summary>
	public class CreatedDateBehavior : IContextBehavior
	{
		private readonly ITimestampService _timestampService;

		public CreatedDateBehavior(ITimestampService timestampService) => _timestampService = timestampService;

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
