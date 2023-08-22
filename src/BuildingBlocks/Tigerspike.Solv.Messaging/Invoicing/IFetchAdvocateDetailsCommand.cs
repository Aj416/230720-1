using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchAdvocateDetailsCommand
	{
		/// <summary>
		/// Advocate identifier.
		/// </summary>
		Guid AdvocateId { get; set; }

		/// <summary>
		/// Brand identifier.
		/// </summary>
		Guid BrandId { get; set; }
	}
}
