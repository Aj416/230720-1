using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Services.Invoicing.Context;
using Tigerspike.Solv.Services.Invoicing.Domain;
using Tigerspike.Solv.Services.Invoicing.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.Invoicing.Infrastructure.Repositories
{
	public class PaymentRepository : Repository<Payment>, IPaymentRepository
	{
		public PaymentRepository(InvoicingDbContext dbContext) : base(dbContext) { }

		public async Task<DateTime?> GetFirstDate(Guid brandId, Guid advocateId)
		{
			return await Queryable()
				.Where(x => x.AdvocateInvoiceLineItem.BrandId == brandId)
				.Where(x => x.AdvocateInvoiceLineItem.AdvocateInvoice.AdvocateId == advocateId)
				.Select(x => (DateTime?)x.CreatedDate)
				.MinAsync();
		}

		/// <inheritdoc/>
		public async Task<DateTime?> GetFirstDate(Guid brandId)
		{
			return await Queryable()
				.Where(x => x.AdvocateInvoiceLineItem.BrandId == brandId)
				.Select(x => (DateTime?)x.CreatedDate)
				.MinAsync();
		}

		/// <inheritdoc/>
		public async Task<DateTime?> GetLastPaymentDate(Guid advocateId)
		{
			return await Queryable()
				.Where(x => x.AdvocateInvoiceLineItem.AdvocateInvoice.AdvocateId == advocateId)
				.MaxAsync(x => (DateTime?)x.CreatedDate);
		}
	}
}
