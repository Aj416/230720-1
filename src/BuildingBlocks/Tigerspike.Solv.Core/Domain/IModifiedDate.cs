using System;

namespace Tigerspike.Solv.Core.Domain
{
	public interface IModifiedDate
	{
		/// <summary>
		/// Modification timestamp of an entity (also set on creation)
		/// </summary>
		DateTime ModifiedDate { get; set; }
	}
}
