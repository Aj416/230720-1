using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	/// <summary>
	/// The brand metadata access repository
	/// </summary>
	public class BrandMetadataAccessRepository : Repository<BrandMetadataAccess>, IBrandMetadataAccessRepository
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext"></param>
		public BrandMetadataAccessRepository(DbContext dbContext) : base(dbContext) { }

		/// <inheritdoc />
		public async Task<List<BrandMetadataAccess>> GetAccessDetails(Guid brandId) 
		{
			return await Queryable()
				.Where(x => x.BrandId == brandId)
				.Include(x => x.Brand)
				.ToListAsync();
		}
	}
}