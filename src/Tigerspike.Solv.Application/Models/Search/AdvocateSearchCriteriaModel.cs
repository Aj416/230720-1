using System;
using Tigerspike.Solv.Core.Models.Search;

namespace Tigerspike.Solv.Application.Models.Search
{
	/// <summary>
	/// A model that represents a search criteria for advocates index.
	/// </summary>
	public class AdvocateSearchCriteriaModel : SearchBaseCriteriaModel
	{
		/// <summary>
		/// Comma separated list of statuses (can be status number or text)
		/// </summary>
		public string Statuses { get; set; }

		/// <summary>
		/// The brand id to filter the advocates by.
		/// </summary>
		public Guid? BrandId { get; set; }

		/// <summary>
		/// The column to sort the results by.
		/// </summary>
		public AdvocateSortBy SortBy { get; set; }
	}

	/// <summary>
	/// The list of available sort columns
	/// </summary>
	public enum AdvocateSortBy
	{
		unspecified,
		fullName,
		csat,
		status,
		onboarding,
		unAuthorisedBrandNames,
		invitedStatus,
		blockBrandNames
	}
}