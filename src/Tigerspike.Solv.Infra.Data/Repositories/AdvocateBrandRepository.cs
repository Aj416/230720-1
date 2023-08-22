using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class AdvocateBrandRepository : Repository<AdvocateBrand>, IAdvocateBrandRepository
	{
		public AdvocateBrandRepository(SolvDbContext context) : base(context)
		{

		}

		/// <inheritdoc/>
		public async Task<int> GetIdleCountByBrand(Guid brandId)
		{
			return await Queryable()
				.Where(x => x.BrandId == brandId)
				.Where(x => x.Enabled == false)
				.Where(x => x.User.Enabled)
				.CountAsync();
		}

		/// <inheritdoc/>
		public async Task<int> GetAuthorizedCountByBrand(Guid brandId)
		{
			return await Queryable()
				.Where(x => x.BrandId == brandId)
				.Where(x => x.Authorized)
				.Where(x => x.Enabled)
				.Where(x => x.User.Enabled)
				.CountAsync();
		}

		/// <inheritdoc/>
		public async Task<int> GetBlockedCountByBrand(Guid brandId)
		{
			return await Queryable()
				.Where(x => x.BrandId == brandId)
				.Where(x => x.User.Enabled == false)
				.CountAsync();
		}

		/// <inheritdoc/>
		public async Task<int> GetTotalCountForBrand(Guid brandId)
		{
			return await Queryable()
				.Where(x => x.BrandId == brandId)
				.CountAsync();
		}

		public Task<List<AdvocateBrand>> GetAllForCsvExport()
		{
			return Queryable()
				.Include(i => i.Brand)
				.OrderBy(o => o.AdvocateId)
				.ToListAsync();
		}
	}
}