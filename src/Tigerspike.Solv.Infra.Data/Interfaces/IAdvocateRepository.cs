using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Models.Induction;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface IAdvocateRepository : IRepository<Advocate>
	{
		/// <summary>
		/// Returns the full graph of an advocate, including all dependencies with tracking enabled.
		/// Mostly used for domain logic.
		/// </summary>
		/// <returns>The full graph ticket</returns>
		Task<Advocate> GetFullAdvocateAsync(Expression<Func<Advocate, bool>> predicate);

		/// <summary>
		/// Gets the status of the induction for the different section items.
		/// </summary>
		/// <param name="advocateId">The advocate Id.</param>
		/// <param name="brandId">The brand Id.</param>
		/// <returns>The list of advocate section items.</returns>
		Task<List<AdvocateSectionItem>> GetInductionSectionItems(Guid advocateId, Guid brandId);

		/// <summary>
		/// Return the list of advocate ids to generate invoices.
		/// </summary>
		Task<IEnumerable<Guid>> GetAllAdvocateIdsForInvoicing();

		/// <summary>
		/// Return list of brand identifiers to be included on the invoice
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		Task<IEnumerable<Guid>> GetBrandIdsForInvoicing(Guid advocateId);

		/// <summary>
		/// Return list of brand identifiers to be included on the graph
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		Task<IEnumerable<Guid>> GetBrandIdsForGraph(Guid advocateId);

		/// <summary>
		/// Return list of brand identifiers with name to be included on the invoice
		/// </summary>
		/// <param name="advocateId">Advocae identifier.</param>
		/// <returns></returns>
		Task<IEnumerable<(Guid id, string name)>> GetBrandDetailsForInvoicing(Guid advocateId);

		/// <summary>
		/// Return the paged list of advocates for indexing
		/// </summary>
		/// <param name="pageIndex">The index of page</param>
		/// <param name="pageSize">The size of page. Default size is 20</param>
		/// <returns>The paged list of Advocate instances</returns>
		Task<IPagedList<Advocate>> GetAdvocatesForIndexing(int pageIndex, int pageSize = 10);
	}
}