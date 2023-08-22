using System;

namespace Tigerspike.Solv.Messaging.WebHook
{
	public interface IDeleteSubscriptionCommand
	{
			/// <summary>
			/// Id of the webhook
			/// </summary>
			public Guid Id { get; set; }

			/// <summary>
			/// The brand id of the webhook to be deleted - so only the owner can delete it
			/// </summary>
			/// <value></value>
			public Guid BrandId { get; set; }
	}
}