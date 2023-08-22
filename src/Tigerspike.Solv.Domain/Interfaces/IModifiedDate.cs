using System;

namespace Tigerspike.Solv.Domain.Interfaces
{
	public interface IModifiedDate
	{
		/// <summary>
		/// Modification timestamp of an entity (also set on creation)
		/// </summary>
		DateTime ModifiedDate { get; set; }
	}
}