using System;
using ServiceStack.DataAnnotations;

namespace Tigerspike.Solv.Chat.Domain
{
	public class MessageWhitelist
	{
		[HashKey]
		public string BrandId { get; set; }

		[RangeKey]
		public string MessageId { get; set; }

		public string Phrase { get; set; }

		public DateTime CreatedDate { get; set; }

		public MessageWhitelist() {}

		public MessageWhitelist(Guid brandId, string phrase)
		{
			BrandId = brandId.ToString();
			Phrase = phrase.ToLowerInvariant();
			MessageId = Guid.NewGuid().ToString();
			CreatedDate = DateTime.UtcNow;
		}
	}
}