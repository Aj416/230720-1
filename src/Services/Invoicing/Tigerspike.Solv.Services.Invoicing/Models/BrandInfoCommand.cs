using System;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class BrandInfoCommand : IBrandInfoCommand
	{
		public Guid BrandId { get; set; }

		public BrandInfoCommand(Guid brandId) => BrandId = brandId;
	}
}
