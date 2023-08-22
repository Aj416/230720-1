using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface IApiKeyRepository : IRepository<ApiKey>
	{
		/// <summary>
		/// Validates specified Api Key.
		/// </summary>
		/// <param name="apiKey">The Api Key</param>
		/// <returns>True if valid, false if revoked, null if not found</returns>
		Task<bool?> IsValidApiKey(string apiKey);

		/// <summary>
		/// Validates the specified application id.
		/// </summary>
		/// <param name="applicationId">The application id.</param>
		/// <returns>True if valid, false if revoked, null if not found</returns>
		Task<bool?> IsValidApplicationId(string applicationId);

		/// <summary>
		/// Gets brand id for the specified api key.
		/// </summary>
		/// <param name="apiKey">The Api Key</param>
		/// <returns>Brand id if found, null otherwise</returns>
		Task<Guid?> GetBrandIdFromApiKey(string apiKey);

		/// <summary>
		/// Gets the brand id for the specified application id.
		/// </summary>
		/// <param name="applicationId">The application id.</param>
		/// <returns>Brand id if found, null otherwise</returns>
		Task<Guid?> GetBrandIdFromApplicationId(string applicationId);
	}
}