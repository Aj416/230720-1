using System;

namespace Tigerspike.Solv.Domain.Models
{
	public class AdvocateApplicationBrand
	{
		public Guid BrandId { get; set; }

		public Guid AdvocateApplicationId { get; set; }

		public Brand Brand { get; set; }

		public AdvocateApplication AdvocateApplication { get; set; }

		public AdvocateApplicationBrand(Guid advocateApplicationId, Guid brandId)
		{
			AdvocateApplicationId = advocateApplicationId;
			BrandId = brandId;
		}
	}
}