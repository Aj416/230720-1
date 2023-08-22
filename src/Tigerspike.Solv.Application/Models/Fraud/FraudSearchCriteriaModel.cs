using System;
using System.ComponentModel.DataAnnotations;
using Tigerspike.Solv.Core.Models.Search;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Fraud
{
	/// <summary>
	/// FraudSearchCriteriaModel.
	/// </summary>
	public class FraudSearchCriteriaModel : SearchBaseCriteriaModel
	{
		/// <summary>
		/// Filters the tickets by the brand speified.
		/// </summary>
		public Guid? BrandId { get; set; }

		/// <summary>
		/// Comma separated list of statuses (can be status number or text)
		/// </summary>
		[Required]
		public FraudStatus? Statuses { get; set; }

		/// <summary>
		/// Comma separated list of statuses (can be status number or text)
		/// </summary>
		public string Risks { get; set; }

		/// <summary>
		/// The list of available sort columns
		/// </summary>
		public FraudSortBy SortBy { get; set; }

		/// <summary>
		/// Level of the ticket
		/// </summary>
		public string Level { get; set; }
	}
}