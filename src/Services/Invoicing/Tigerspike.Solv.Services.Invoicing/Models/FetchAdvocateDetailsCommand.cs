using System;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class FetchAdvocateDetailsCommand : IFetchAdvocateDetailsCommand
	{
		/// <inheritdoc />
		public Guid AdvocateId { get; set; }

		/// <inheritdoc />
		public Guid BrandId { get; set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="advocateId">Advocate identifier.</param>
		/// <param name="brandId">Brand identifier.</param>
		public FetchAdvocateDetailsCommand(Guid advocateId, Guid brandId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
		}
	}
}
