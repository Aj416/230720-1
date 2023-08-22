using System.Collections.Generic;

namespace Tigerspike.Solv.Core.Models
{
	public class PaginatedResult<T>
	{
		public List<T> Items { get; set; }

		public long ItemsTotal { get; set; }

		public int Page { get; set; }

		public int ItemsPerPage { get; set; }
	}
}