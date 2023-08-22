using System;

namespace Tigerspike.Solv.Api.Models
{
	/// <summary>
	/// Advocate Performance Request Model
	/// </summary>
	public class AdvocatePerformanceRequestModel
	{
		/// <summary>
		/// From timestamp
		/// </summary>
		public DateTime? From { get; set; }

		/// <summary>
		/// The brand Id
		/// </summary>
		public Guid[] BrandIds { get; set; }

	}
}
