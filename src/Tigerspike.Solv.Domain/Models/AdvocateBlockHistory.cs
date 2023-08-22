using System;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Domain.Models
{
	public class AdvocateBlockHistory
	{
		/// <summary>
		/// Constructor to please EF.
		/// </summary>
		private AdvocateBlockHistory() { }

		public Guid Id { get; private set; }
		public Guid AdvocateId { get; private set; }
		public Guid BrandId { get; private set; }
		public DateTime CreatedDate {get; set; }
		public Brand Brand { get; private set; }

		public AdvocateBlockHistory(Guid brandId, Guid advocateId, DateTime createdDate)
		{
			BrandId = brandId;
			AdvocateId = advocateId;
			CreatedDate = createdDate;
		}
	}
}