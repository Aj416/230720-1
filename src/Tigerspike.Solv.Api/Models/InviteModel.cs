using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Api.Models
{
	/// <summary>
	/// Model for bulk invite
	/// </summary>
	public class InviteModel : BulkActionModel
	{
		/// <summary>
		/// List of selected brands to invite
		/// </summary>
		public IEnumerable<Guid> Brands { get; set; }

	}
}