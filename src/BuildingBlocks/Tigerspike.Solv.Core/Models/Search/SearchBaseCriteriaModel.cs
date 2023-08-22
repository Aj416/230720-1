using System;
using Tigerspike.Solv.Core.Models.PagedList;

namespace Tigerspike.Solv.Core.Models.Search
{
	public class SearchBaseCriteriaModel : PagedRequestModel
	{
		public string Term { get; set; }

		public DateTime? From { get; set; }

		public DateTime? To { get; set; }

		public SortOrder SortOrder { get; set; } = SortOrder.Desc;
	}
}