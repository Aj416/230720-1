using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Infra.Data.Models
{

	public class BrandResponseConfigModel
	{
		/// <summary>
		/// The brand identifier.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Response type
		/// </summary>
		public BrandAdvocateResponseType ResponseType { get; set; }

		/// <summary>
		/// Delay after which response should be sent
		/// </summary>
		public int? DelayInSeconds { get; set; }

		/// <summary>
		/// Template content of the response
		/// </summary>
		public string Content { get; set; }


		public BrandResponseConfigModel(Guid brandId, BrandAdvocateResponseType responseType, int? delayInSeconds, string content)
		{
			BrandId = brandId;
			ResponseType = responseType;
			DelayInSeconds = delayInSeconds;
			Content = content;
		}
	}
}