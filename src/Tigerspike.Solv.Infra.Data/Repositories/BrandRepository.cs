using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Models.Induction;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class BrandRepository : Repository<Brand>, IBrandRepository
	{
		public BrandRepository(SolvDbContext context) : base(context) { }

		/// <inheritdoc />
		public async Task<IEnumerable<AdvocateBrand>> GetBrands(Guid advocateId, bool? isPractice = null)
		{
			var result = await Queryable()
				.Where(x => !isPractice.HasValue || x.IsPractice == isPractice)
				.Include(x => x.Advocates)
				.ThenInclude(x => x.Brand)
				.SelectMany(x => x.Advocates)
				.Where(x => x.AdvocateId == advocateId)
				.ToListAsync();

			return result;
		}

		/// <inheritdoc/>
		public Task<List<Guid>> GetBrandsForAdvocateApplication(Guid advocateId)
		{
			var query = from advAppBrand in DbContext.Set<AdvocateApplicationBrand>()
						where advAppBrand.AdvocateApplicationId == advocateId
						select advAppBrand.BrandId;

			return query.ToListAsync();
		}
		/// <inheritdoc/>
		public async Task<IEnumerable<Brand>> GetAll()
		{
			return await Queryable()
				.Where(x => x.IsPractice == false)
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<Tag>> GetTags(Guid brandId, bool activeOnly = true, TicketLevel? level = null)
		{
			var query = Queryable()
				.Where(x => x.Id == brandId)
				.SelectMany(x => x.Tags);

			query = activeOnly ? query.Where(x => x.Enabled) : query;
			query = level.HasValue ? query.Where(x => x.Level == null || x.Level <= (int)level) : query;

			return await query.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<Guid>> GetBrandIdsForInvoicing()
		{
			var query = from brand in Queryable()
						where !brand.IsPractice && brand.InvoicingEnabled
						select brand.Id;
			return await query.ToListAsync();
		}

		/// <inheritdoc/>
		public Task<List<Section>> GetInductionSections(Guid brandId)
		{
			var query = from section in DbContext.Set<Section>().Include(s => s.SectionItems)
						where section.BrandId == brandId && section.Enabled
						orderby section.Order
						select new Section(section.Id, section.Name, section.Enabled, section.BrandId, section.Order,
							section.SectionItems.Where(si => si.Enabled).ToList());

			return query.ToListAsync();
		}

		/// <inheritdoc />
		public Task<SectionItem> GetInductionSectionItem(Guid sectionItemId) => DbContext.Set<SectionItem>().Where(si => si.Id == sectionItemId).SingleOrDefaultAsync();

		/// <inheritdoc/>
		public Task<List<SectionItem>> GetInductionSectionItems(Guid brandId)
		{
			return DbContext.Set<SectionItem>().Include(si => si.Section)
				.Where(si => si.Section.BrandId == brandId && si.Enabled)
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<bool> AreAssignable(Guid[] brandIds)
		{
			var validCount = await Queryable()
				.Where(x => x.IsPractice == false)
				.Where(x => brandIds.Contains(x.Id))
				.CountAsync();
			return validCount == brandIds.Length;
		}

		/// <inheritdoc/>
		public Task<IEnumerable<BrandNotificationConfig>> GetNotificationConfig(Guid brandId, BrandNotificationType type, bool activeOnly = true) => GetNotificationConfig(brandId, new[] { type }, activeOnly);

		/// <inheritdoc/>
		public async Task<IEnumerable<BrandNotificationConfig>> GetNotificationConfig(Guid brandId, BrandNotificationType[] types, bool activeOnly = true)
		{
			var query = Queryable()
				.Where(x => x.Id == brandId)
				.SelectMany(x => x.NotificationConfig)
				.Where(x => types.Contains(x.Type));

			query = activeOnly ? query.Where(x => x.IsActive) : query;
			return await query.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<IList<Guid>> GetActiveBrandsIds(Guid advocateId) => await Queryable()
			.SelectMany(s => s.Advocates)
			.Where(x => x.AdvocateId == advocateId)
			.Where(x => x.Enabled && !x.Blocked)
			.Select(s => s.BrandId)
			.ToListAsync();

		/// <inheritdoc/>
		public async Task<IEnumerable<Category>> GetCategories(Guid brandId, bool isEnabled = true)
		{
			return await Queryable()
				.Where(x => x.Id == brandId)
				.SelectMany(x => x.Categories)
				.Where(c => c.Enabled == isEnabled)
				.OrderBy(c => c.Order)
				.ToListAsync();
		}
	}
}