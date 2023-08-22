using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	/// <summary>
	/// The brand metadata routing config repository
	/// </summary>
	public interface IBrandMetadataRoutingConfigRepository
	{
		/// <summary>
		/// Method to fetch the routing config for the brand based on metadata item
		/// </summary>
		/// <param name="brandId"></param>
		/// <returns></returns>
		Task<BrandMetadataRoutingConfig> GetRoutingConfig(Guid brandId);
	}
}