using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Services.Invoicing.Context;
using Tigerspike.Solv.Services.Invoicing.Domain;
using Tigerspike.Solv.Services.Invoicing.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.Invoicing.Infrastructure.Repositories
{
	public class AdvocateInvoiceRepository : Repository<AdvocateInvoice>, IAdvocateInvoiceRepository
	{
		public AdvocateInvoiceRepository(InvoicingDbContext solvDbContext)
			: base(solvDbContext) { }

	}
}
