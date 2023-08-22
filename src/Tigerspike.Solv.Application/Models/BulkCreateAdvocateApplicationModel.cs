using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Bulk advocate application creation request
	/// </summary>
	public class BulkCreateAdvocateApplicationModel
	{
		/// <summary>
		/// List of applications to create
		/// </summary>
		[MaxLength(100)]
		public IList<AdvocateApplicationModel> Applications { get; set; }

		/// <summary>
		/// List of pre-assigned brands for the applicants
		/// </summary>
		public IList<Guid> Brands { get; set; }

		/// <summary>
		/// Source to be set on the applications
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Whether applications should be for internal agents
		/// </summary>
		public bool InternalAgent { get; set; }
	}
}