using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	/// <summary>
	/// The metadata routing config repository
	/// </summary>
	public class BrandMetadataRoutingConfigRepository : Repository<BrandMetadataRoutingConfig>, IBrandMetadataRoutingConfigRepository
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext"></param>
		public BrandMetadataRoutingConfigRepository(DbContext dbContext) : base(dbContext) { }

		/// <inheritdoc />
		public async Task<BrandMetadataRoutingConfig> GetRoutingConfig(Guid brandId) 
		{
			return await Queryable()
				.Where(x => x.BrandId == brandId)
				.Include(x => x.Brand)
				.FirstOrDefaultAsync();
		}
	}
}