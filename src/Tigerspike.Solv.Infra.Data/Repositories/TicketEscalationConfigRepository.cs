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
	public class TicketEscalationConfigRepository : Repository<TicketEscalationConfig>, ITicketEscalationConfigRepository
	{
		public TicketEscalationConfigRepository(SolvDbContext dbContext) : base(dbContext) { }

		/// <inheritdoc/>
		public async Task<TicketEscalationConfig> Get(Guid brandId, int? ticketSourceId)
		{
			return await Queryable()
				.Where(x => x.BrandId == brandId)
				.Where(x => x.TicketSourceId == ticketSourceId)
				.FirstOrDefaultAsync();
		}

		/// <inheritdoc/>
		public async Task<IList<Guid>> GetBrands()
		{
			return await Queryable()
				.Select(x => x.BrandId)
				.Distinct()
				.ToListAsync();
		}
	}
}