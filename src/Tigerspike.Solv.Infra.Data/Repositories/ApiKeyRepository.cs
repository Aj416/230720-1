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
	public class ApiKeyRepository : Repository<ApiKey>, IApiKeyRepository
	{
		public ApiKeyRepository(SolvDbContext dbContext) : base(dbContext) { }

		/// <inheritdoc/>
		public async Task<bool?> IsValidApiKey(string apiKey)
		{
			var entry = await Queryable().FirstOrDefaultAsync(x => x.Key == apiKey);
			return entry != null ? (bool?)(entry.RevokedDate == null) : null;
		}

		/// <inheritdoc/>
		public async Task<bool?> IsValidApplicationId(string applicationId)
		{
			var entry = await Queryable().FirstOrDefaultAsync(x => x.ApplicationId == applicationId);
			return entry != null ? (bool?)(entry.RevokedDate == null) : null;
		}

		/// <inheritdoc/>
		public async Task<Guid?> GetBrandIdFromApiKey(string apiKey)
		{
			return await Queryable()
				.Where(x => x.Key == apiKey)
				.Select(x => (Guid?)x.BrandId)
				.FirstOrDefaultAsync();
		}

		/// <inheritdoc/>
		public async Task<Guid?> GetBrandIdFromApplicationId(string applicationId)
		{
			return await Queryable()
				.Where(x => x.ApplicationId == applicationId)
				.Select(x => (Guid?)x.BrandId)
				.FirstOrDefaultAsync();
		}
	}
}