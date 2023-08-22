using Tigerspike.Solv.Core.Models.Search;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Search
{
	public class AdvocateApplicationSearchCriteriaModel : SearchBaseCriteriaModel
	{
		/// <summary>
		/// List of countries to filter in search, comma separated
		/// </summary>
		public string Countries { get; set; }

		/// <summary>
		/// List of countries to filter in search, comma separated
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// List of countries to filter in search, comma separated
		/// </summary>
		public string Phone { get; set; }

		/// <summary>
		/// Candidates will be searched on basis of status - Enum of type AdvocateApplicationStatus.
		/// </summary>
		public AdvocateApplicationStatus? Statuses { get; set; }

		/// <summary>
		/// The list of available sort columns.
		/// </summary>
		public AdminAdvocateApplicationStatusSortBy SortBy { get; set; }
	}
}