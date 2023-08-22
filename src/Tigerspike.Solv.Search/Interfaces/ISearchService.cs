using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Models.Search;
using Tigerspike.Solv.Search.Models;

namespace Tigerspike.Solv.Search.Interfaces
{
	public interface ISearchService<T> where T : class
	{
		Task<IPagedList> Search(SearchBaseCriteriaModel searchCriteria);

		Task Index(T item);

		/// <summary>
		/// Pass an anonymous object with only the fields that needs to be updated.
		/// </summary>
		Task Update(Guid id, object partialItem);

		Task Delete(Guid id);
		Task<RebuildResult> Rebuild();
	}
}
