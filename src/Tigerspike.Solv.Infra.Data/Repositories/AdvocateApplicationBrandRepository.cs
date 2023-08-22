using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class AdvocateApplicationBrandRepository : Repository<AdvocateApplicationBrand>, IAdvocateApplicationBrandRepository
	{
		public AdvocateApplicationBrandRepository(SolvDbContext context) : base(context) { }
	}
}