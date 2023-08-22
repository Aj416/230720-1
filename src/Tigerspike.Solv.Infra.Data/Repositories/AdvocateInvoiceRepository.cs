using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class AdvocateInvoiceRepository : Repository<AdvocateInvoice>, IAdvocateInvoiceRepository
	{
		public AdvocateInvoiceRepository(SolvDbContext solvDbContext)
			: base(solvDbContext) { }

	}
}
