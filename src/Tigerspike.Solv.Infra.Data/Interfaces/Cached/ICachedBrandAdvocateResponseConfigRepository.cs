using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces.Cached
{
	public interface ICachedBrandAdvocateResponseConfigRepository
	{
		Task<IEnumerable<BrandAdvocateResponseConfig>> Get(Guid brandId);
	}
}