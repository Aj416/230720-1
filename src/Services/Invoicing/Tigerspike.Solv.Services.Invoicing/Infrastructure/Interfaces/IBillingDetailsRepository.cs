using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Services.Invoicing.Domain;

namespace Tigerspike.Solv.Services.Invoicing.Infrastructure.Interfaces
{
	public interface IBillingDetailsRepository : IRepository<BillingDetails>
	{
		/// <summary>
		/// Returns current billing details id of the platform
		/// </summary>
		Task<Guid> GetCurrentIdForPlatform();
	}
}
