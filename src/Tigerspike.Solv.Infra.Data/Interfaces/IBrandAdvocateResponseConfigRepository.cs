using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface IBrandAdvocateResponseConfigRepository : IRepository<BrandAdvocateResponseConfig>
	{
		/// <summary>
		/// Return list of responses for particular brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		Task<IEnumerable<BrandAdvocateResponseConfig>> Get(Guid brandId);

		/// <summary>
		/// Returns the requested response for a particular brand 
		/// </summary>
		/// <param name="brandId"></param>
		/// <param name="brandAdvocateResponseType"></param>
		/// <returns></returns>
		Task<BrandAdvocateResponseConfig> Get(Guid brandId, BrandAdvocateResponseType brandAdvocateResponseType);
	}
}