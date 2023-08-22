using System;
using System.ComponentModel.DataAnnotations;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Core.Models.Search
{
	public class TicketSearchCriteriaModel : SearchBaseCriteriaModel
	{
		/// <summary>
		/// The advocate Id that we need to filter the tickets by.
		/// </summary>
		public Guid? AdvocateId { get; set; }

		/// <summary>
		/// Filters the tickets by the brand speified.
		/// </summary>
		public Guid? BrandId { get; set; }

		/// <summary>
		/// Comma separated list of statuses (can be status number or text)
		/// </summary>
		[Required]
		public string Statuses { get; set; }

		/// <summary>
		/// The list of available sort columns
		/// </summary>
		public TicketSortBy SortBy { get; set; }

		/// <summary>
		/// Level of the ticket
		/// </summary>
		public TicketLevel? Level { get; set; }
	}

	/// <summary>
	/// The list of available sort columns
	/// </summary>
	public enum TicketSortBy
	{
		unspecified,
		id,
		advocateFullName,
		createdDate,
		closedDate,
		escalatedDate,
		question,
		csat,
		complexity,
		absoluteTimeToClose,
		solverTimeToClose,
		timeOpen,
		price,
		source,
		referenceId,
		escalationReasonText,
		brandName,
	}
}
