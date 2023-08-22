using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface IAdvocateBrandRepository : IRepository<AdvocateBrand>
	{
		/// <summary>
		/// Gets the count of idle advocates for a specific brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <returns>The number of idle advocates.</returns>
		Task<int> GetIdleCountByBrand(Guid brandId);

		/// <summary>
		/// Gets the count of authorised advocates for a specific brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <returns>The number of authorized advocates.</returns>
		Task<int> GetAuthorizedCountByBrand(Guid brandId);

		/// <summary>
		/// Gets the count of blocked advocates for a specific brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <returns>The number of blocked advocates.</returns>
		Task<int> GetBlockedCountByBrand(Guid brandId);

		/// <summary>
		/// Gets the total count of advocates for a specific brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <returns>The number of advocates.</returns>
		Task<int> GetTotalCountForBrand(Guid brandId);

		/// <summary>
		/// Returns all advocate brands in db (for the purpuse of CSV generating mainly)
		/// </summary>
		Task<List<AdvocateBrand>> GetAllForCsvExport();
	}
}