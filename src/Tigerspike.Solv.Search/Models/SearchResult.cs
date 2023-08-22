using System.Collections.Generic;

namespace Tigerspike.Solv.Search.Models
{
	public class SearchResult<T> where T : class
	{
		/// <summary>
		/// The search result items in the current page.
		/// </summary>
		public List<T> Items { get; set; }

		/// <summary>
		/// The total matches number.
		/// </summary>
		public long TotalMatches { get; set; }
	}
}
