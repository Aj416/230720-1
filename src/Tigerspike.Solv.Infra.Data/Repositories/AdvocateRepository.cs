using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Models.Induction;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class AdvocateRepository : Repository<Advocate>, IAdvocateRepository
	{
		public AdvocateRepository(SolvDbContext dbContext) : base(dbContext) { }

		/// <inheritdoc />
		public async Task<Advocate> GetFullAdvocateAsync(Expression<Func<Advocate, bool>> predicate)
		{
			var query = Queryable()
				.Where(predicate);

			var advocate = await query
				.Include(x => x.User)
				.FirstOrDefaultAsync();

			if (advocate != null)
			{
				await query
					.Include(ii => ii.Brands)
					.ThenInclude(ab => ab.Brand)
					.ThenInclude(b => b.InductionSections)
					.ThenInclude(s => s.SectionItems)
					.ThenInclude(si => si.AdvocateSectionItems)
					.LoadAsync();

				await query
					.Include(a => a.QuizAttempts)
					.LoadAsync();

				await query
					.Include(a => a.BlockHistory)
					.ThenInclude(bb => bb.Brand)
					.LoadAsync();
			}

			return advocate;
		}

		/// <inheritdoc />
		public Task<List<AdvocateSectionItem>> GetInductionSectionItems(Guid advocateId, Guid brandId)
		{
			return GetContext<SolvDbContext>().InductionAdvocateSectionItems
				.Include(asi => asi.SectionItem)
					.ThenInclude(si => si.Section)
				.Where(asi => asi.SectionItem.Section.BrandId == brandId && asi.AdvocateId == advocateId)
				.OrderBy(x => x.SectionItem.Order)
				.ToListAsync();
		}

		/// <inheritdoc />
		public async Task<IEnumerable<Guid>> GetAllAdvocateIdsForInvoicing()
		{
			var query = from advocate in Queryable()
						where advocate.Brands.Any(b => b.Authorized && !b.Brand.IsPractice)
						select advocate.Id;
			return await query.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<Guid>> GetBrandIdsForInvoicing(Guid advocateId)
		{
			return await (
					from ab in DbContext.Set<AdvocateBrand>()
					join brand in DbContext.Set<Brand>() on ab.BrandId equals brand.Id
					where ab.AdvocateId == advocateId && ab.Enabled && brand.IsPractice == false
					select ab.BrandId
				)
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<Guid>> GetBrandIdsForGraph(Guid advocateId)
		{
			return await (
					from ab in DbContext.Set<AdvocateBrand>()
					join brand in DbContext.Set<Brand>() on ab.BrandId equals brand.Id
					where ab.AdvocateId == advocateId && brand.IsPractice == false && ab.Authorized
					select ab.BrandId
				)
				.ToListAsync();
		}

		public async Task<IEnumerable<(Guid id, string name)>> GetBrandDetailsForInvoicing(Guid advocateId)
		{
			var result = await (
				from b in DbContext.Set<Brand>()
				join ab in DbContext.Set<AdvocateBrand>() on b.Id equals ab.BrandId
				where ab.AdvocateId == advocateId && ab.Enabled && b.IsPractice == false
				select new { id = b.Id, name = b.Name }
			).ToListAsync();

			return result
			.Select(x => (x.id, x.name));
		}

		
		public async Task<IPagedList<Advocate>> GetAdvocatesForIndexing(int pageIndex, int pageSize = 10)
		{
			var baseQuery = Queryable()
				.AsNoTracking();
			
			// This is a temperory workaround. Ideally the AdvocateSectionItems should be excluded 
			// and the induction logic for induction statuses should be re-written
			var pagedList = baseQuery.ToPagedList(pageIndex, pageSize);

			var query = Queryable()
				.Where(x => pagedList.Items.Select(y => y.Id).Contains(x.Id));
			
			await query
				.Include(x => x.User)
				.Include(x => x.QuizAttempts)
				.LoadAsync();
			
			await query
				.Include(ii => ii.Brands)	
				.ThenInclude(ab => ab.Brand)
				.ThenInclude(b => b.InductionSections)
				.ThenInclude(s => s.SectionItems)
				.ThenInclude(si => si.AdvocateSectionItems)
				.LoadAsync();
			
			return PagedList.FromExisting(query.ToList(), pageIndex, pageSize, pagedList.TotalCount, 0);
		}
	}
}