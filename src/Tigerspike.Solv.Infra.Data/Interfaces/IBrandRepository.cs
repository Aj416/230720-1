using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Models.Induction;
using Tigerspike.Solv.Infra.Data.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface IBrandRepository : IRepository<Brand>
	{
		/// <summary>
		/// Returns the list of a selected brands to be linked to an advocate.
		/// The selected brands were chosen at the advocate application level
		/// before inviting the solver
		/// </summary>
		Task<List<Guid>> GetBrandsForAdvocateApplication(Guid advocateId);

		/// <summary>
		/// Return list of brands of the advocate
		/// </summary>
		/// <param name="isPractice">Filter practice brands, pass null to fetch all regardless</param>
		Task<IEnumerable<AdvocateBrand>> GetBrands(Guid advocateId, bool? isPractice = null);

		/// <summary>
		/// Gets all the brand ids with invoicing enabled.
		/// </summary>
		/// <returns>The brand ids.</returns>
		Task<IEnumerable<Guid>> GetBrandIdsForInvoicing();

		/// <summary>
		/// Get tags for brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="activeOnly">To select only active tags or all of them</param>
		/// <param name="level">The ticket level</param>
		Task<IEnumerable<Tag>> GetTags(Guid brandId, bool activeOnly = true, TicketLevel? level = null);

		/// <summary>
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <returns>The induction section items.</returns>
		Task<List<Section>> GetInductionSections(Guid brandId);

		/// <summary>
		/// Gets an induction section item by id.
		/// </summary>
		/// <param name="sectionItemId">section item id.</param>
		/// <returns>The induction section item.</returns>
		Task<SectionItem> GetInductionSectionItem(Guid sectionItemId);

		/// <summary>
		/// Gets all induction section items for a specified brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <returns></returns>
		Task<List<SectionItem>> GetInductionSectionItems(Guid brandId);

		/// <summary>
		/// Validate if the brands exists and advocate applications can be assigned to them
		/// </summary>
		/// <param name="brandIds">The list of ids to be validated</param>
		Task<bool> AreAssignable(Guid[] brandIds);

		/// <summary>
		/// Gets notification config for the specified event for the brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="type">The notification type</param>
		/// <param name="activeOnly">To select only active configs or all of them</param>
		Task<IEnumerable<BrandNotificationConfig>> GetNotificationConfig(Guid brandId, BrandNotificationType type, bool activeOnly = true);

		/// <summary>
		/// Gets notification config for the specified event for the brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="types">The notification types</param>
		/// <param name="activeOnly">To select only active configs or all of them</param>
		Task<IEnumerable<BrandNotificationConfig>> GetNotificationConfig(Guid brandId, BrandNotificationType[] types, bool activeOnly = true);

		/// <summary>
		/// Return the list of brands ids associated with the advocate.
		/// </summary>
		Task<IList<Guid>> GetActiveBrandsIds(Guid advocateId);

		/// <summary>
		/// Returns categories specific to brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <param name="isEnabled">IsEnabled flag.</param>
		/// <returns>List of categories</returns>
		Task<IEnumerable<Category>> GetCategories(Guid brandId, bool isEnabled = true);
	}
}