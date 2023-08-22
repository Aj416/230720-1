using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IBrandInfoCommand
	{
		/// <summary>
		/// Brand for which info required.
		/// </summary>
		public Guid BrandId { get; set; }
	}
}
