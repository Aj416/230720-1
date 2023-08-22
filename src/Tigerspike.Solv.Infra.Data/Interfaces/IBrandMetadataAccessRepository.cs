using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	/// <summary>
	/// The brand metadata access repository
	/// </summary>
	public interface IBrandMetadataAccessRepository
	{
		/// <summary>
		/// Method to get all the metadata items access details for a brand
		/// </summary>
		/// <param name="brandId"></param>
		/// <returns></returns>
		Task<List<BrandMetadataAccess>> GetAccessDetails(Guid brandId);
	}
}