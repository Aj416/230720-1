using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.WebHook
{
	public interface IStartWebHookTriggerCommand
	{
		public Guid BrandId { get; set; }

		public int EventType { get; set; }

		public IDictionary<string, object> Payload { get; set; }
	}
}