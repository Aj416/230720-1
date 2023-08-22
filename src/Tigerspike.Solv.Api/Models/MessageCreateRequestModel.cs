using System;

namespace Tigerspike.Solv.Api.Models
{
	public class MessageCreateRequestModel
	{
		public string Message { get; set; }

		public Guid ClientMessageId { get; set; }
	}
}