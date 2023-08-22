using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Infra.Data.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces.Cached
{
	public interface ICachedAdvocateRepository
	{
		/// <summary>
		/// Get the list of cached super solvers info.
		/// </summary>
		Task<List<UserEmailModel>> GetSuperSolvers(Guid brandId);

		/// <summary>
		/// Returns the ids of the online advocates for the specified brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <returns>The list of online advocate ids.</returns>
		Task<List<Guid>> GetOnlineAdvocates(Guid brandId);
		/// <summary>
		///
		/// </summary>
		/// <param name="advocateId"></param>
		/// <returns></returns>
		Task<bool> GetInternalAgentInfo(Guid advocateId);
	}
}