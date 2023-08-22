using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// The brand metadata access
	/// </summary>
	public class BrandMetadataAccess
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
		/// The access granted to ticket level
		/// </summary>
		public TicketLevel Level { get; set; }
	}
}