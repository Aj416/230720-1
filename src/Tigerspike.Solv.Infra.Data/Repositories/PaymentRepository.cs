using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class PaymentRepository : Repository<Payment>, IPaymentRepository
	{
		public PaymentRepository(SolvDbContext dbContext) : base(dbContext) { }

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