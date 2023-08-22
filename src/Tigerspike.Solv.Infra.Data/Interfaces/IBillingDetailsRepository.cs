using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface IBillingDetailsRepository : IRepository<BillingDetails>
	{

		/// <summary>
		/// Returns current billing details id of the platform
		/// </summary>
		Task<Guid> GetCurrentIdForPlatform();

		/// <summary>
		/// Returns current billing details of the platform
		/// </summary>
		Task<BillingDetails> GetCurrentForPlatform();
	}
}