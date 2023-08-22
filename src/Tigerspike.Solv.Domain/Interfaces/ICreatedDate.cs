using System;

namespace Tigerspike.Solv.Domain.Interfaces
{
	public interface ICreatedDate
	{
		/// <summary>
		/// Creation timestamp of an entity
		/// </summary>
		DateTime CreatedDate { get; set; }
	}
}