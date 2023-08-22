using System;

namespace Tigerspike.Solv.Core.Domain
{
	public interface ICreatedDate
	{
		/// <summary>
		/// Creation timestamp of an entity
		/// </summary>
		DateTime CreatedDate { get; set; }
	}
}
