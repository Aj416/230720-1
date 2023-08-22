using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// The metadata routing configuration
	/// </summary>
	public class BrandMetadataRoutingConfig
	{
		/// <summary>
		/// The identifier
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The brand identifier
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The brand
		/// </summary>
		public Brand Brand { get; set; }

		/// <summary>
		/// The metadata field name
		/// </summary>
		public string Field { get; set; }

		/// <summary>
		/// The metadata field value
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// The routing logic to ticket level
		/// </summary>
		public TicketLevel RouteTo { get; set; }
	}
}