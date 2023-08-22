using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class ClientInvoiceRepository : Repository<ClientInvoice>, IClientInvoiceRepository
	{
		public ClientInvoiceRepository(SolvDbContext context) : base(context) { }
	}
}
