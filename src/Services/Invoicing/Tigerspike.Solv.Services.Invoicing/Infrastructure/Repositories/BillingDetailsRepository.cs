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
	public class BillingDetailsRepository : Repository<BillingDetails>, IBillingDetailsRepository
	{
		public BillingDetailsRepository(InvoicingDbContext solvDbContext)
			: base(solvDbContext) { }

		/// <inheritdoc/>
		public async Task<Guid> GetCurrentIdForPlatform()
		{
			return await Queryable()
				.Where(x => x.IsPlatformOwner && !string.IsNullOrEmpty(x.Email))
				.OrderByDescending(x => x.CreatedDate)
				.Select(x => x.Id)
				.FirstAsync();
		}

	}
}
