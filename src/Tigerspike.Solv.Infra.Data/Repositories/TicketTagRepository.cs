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
	public class TicketTagRepository : Repository<TicketTag>, ITicketTagRepository
	{
		public TicketTagRepository(SolvDbContext context) : base(context) { }

		/// <inheritdoc />
		public async Task<IEnumerable<Guid>> GetEscalatedTicketIds(HashSet<Guid> tagIds, Guid advocateId)
		{
			return await Queryable()
				.Where(x => x.UserId == advocateId)
				.Where(x => tagIds.Contains(x.TagId))
				.Select(x => x.TicketId)
				.Distinct()
				.ToListAsync();
		}
	}
}
