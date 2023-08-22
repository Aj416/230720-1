using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class BrandAdvocateResponseConfigRepository : Repository<BrandAdvocateResponseConfig>, IBrandAdvocateResponseConfigRepository
	{
		public BrandAdvocateResponseConfigRepository(SolvDbContext context) : base(context) { }

		/// <inheritdoc/>
		public async Task<IEnumerable<BrandAdvocateResponseConfig>> Get(Guid brandId)
		{
			return await Queryable()
				.Where(x => x.IsActive)
				.Where(x => x.BrandId == null || x.BrandId == brandId) // get brand specific + default responses
				/*.Include(x => x.ChatAction)
					.ThenInclude(x => x.Options)
					.ThenInclude(x => x.SideEffects)*/
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<BrandAdvocateResponseConfig> Get(Guid brandId, BrandAdvocateResponseType brandAdvocateResponseType)
		{
			var query = await Queryable()
					.Where(x => x.IsActive)
					.Where(x => x.BrandId == brandId && x.Type == brandAdvocateResponseType)
					.FirstOrDefaultAsync();

			return query ?? await Queryable()
					.Where(x => x.IsActive)
					.Where(x => x.Type == brandAdvocateResponseType)
					.FirstOrDefaultAsync();
		}
	}
}