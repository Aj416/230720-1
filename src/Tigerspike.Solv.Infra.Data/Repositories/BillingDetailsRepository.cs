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
	public class BillingDetailsRepository : Repository<BillingDetails>, IBillingDetailsRepository
	{
		public BillingDetailsRepository(SolvDbContext dbContext) : base(dbContext) { }

		/// <inheritdoc/>
		public async Task<Guid> GetCurrentIdForPlatform()
		{
			return await Queryable()
				.Where(x => x.IsPlatformOwner && !string.IsNullOrEmpty(x.Email))
				.OrderByDescending(x => x.CreatedDate)
				.Select(x => x.Id)
				.FirstAsync();
		}

		/// <inheritdoc/>
		public async Task<BillingDetails> GetCurrentForPlatform()
		{
			return await Queryable()
				.Where(x => x.IsPlatformOwner && !string.IsNullOrEmpty(x.Email))
				.OrderByDescending(x => x.CreatedDate)
				.FirstAsync();
		}
	}
}