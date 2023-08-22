using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Models.Search;
using Tigerspike.Solv.Services.Fraud.Enum;
using Tigerspike.Solv.Services.Fraud.Models;

namespace Tigerspike.Solv.Services.Fraud.Application.Services
{
	public interface ISearchService<T> where T : class
	{
		/// <summary>
		/// Search records based on specific criteria.
		/// </summary>
		/// <param name="searchCriteria">Criteria on basis of which records searched.</param>
		/// <returns>Paged List search result.</returns>
		Task<IPagedList> Search(SearchBaseCriteriaModel searchCriteria);

		/// <summary>
		/// Index a generic type,
		/// </summary>
		/// <param name="item">Item to be indexed.</param>
		/// <returns></returns>
		Task Index(T item);

		/// <summary>
		/// Pass an anonymous object with only the fields that needs to be updated.
		/// </summary>
		Task Update(Guid id, object partialItem);

		/// <summary>
		/// Delete index for specific identifier.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <returns></returns>
		Task Delete(Guid id);

		/// <summary>
		/// Rebuild index.
		/// </summary>
		/// <returns></returns>
		Task<RebuildResult> Rebuild();

		/// <summary>
		/// Get count of Fraud tickets - FraudSuspected, FraudConfirmed, NotFraudalent etc.
		/// </summary>
		/// <param name="fraudStatus">The fraud status.</param>
		/// <returns>The count of fraud tickets for given fraud status.</returns>
		Task<long> GetCount(FraudStatus fraudStatus);
	}
}
