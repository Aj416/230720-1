using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Infra.Data.Models
{
	/// <summary>
	/// Advocate Performance Breakdown Model.
	/// </summary>
	public class AdvocatePerformanceBreakDownModel
	{
		/// <summary>
		/// Key based on which the graph in UI will be populated.
		/// </summary>
		public DateTime Key { get; set; }

		/// <summary>
		/// Dictionary with brand id as key and total closed ticket price as value for the given Key property.
		/// </summary>
		public Dictionary<Guid, decimal> Series { get; set; }
	}
}