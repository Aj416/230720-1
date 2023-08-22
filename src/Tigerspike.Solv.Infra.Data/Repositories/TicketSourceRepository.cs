using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class TicketSourceRepository : Repository<TicketSource>, ITicketSourceRepository
	{
		public TicketSourceRepository(SolvDbContext dbContext) : base(dbContext) { }
		public Task<TicketSource> Get(string source) => GetFirstOrDefaultAsync(predicate: x => x.Name == source);
	}
}