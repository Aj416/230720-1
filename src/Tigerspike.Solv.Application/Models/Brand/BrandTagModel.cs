using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Brand
{
	/// <summary>
	/// The brand tag model 
	/// </summary>
    public class BrandTagModel
    {
		/// <summary>
		/// The brand id
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The disabled tag ids
		/// </summary>
		public List<Guid> DisabledTags { get; set; }
    }
}