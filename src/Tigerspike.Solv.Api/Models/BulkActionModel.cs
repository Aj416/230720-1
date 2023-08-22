using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Api.Models
{
	/// <summary>
	/// Model for bulk actions in tables
	/// </summary>
	public class BulkActionModel
	{
		/// <summary>
		/// List of selected items
		/// </summary>
		/// <value></value>
		public IEnumerable<Guid> Items { get; set; }
	}
}