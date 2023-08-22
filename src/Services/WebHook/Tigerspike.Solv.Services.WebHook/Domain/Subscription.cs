using System;
using MassTransit;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Services.WebHook.Enums;

namespace Tigerspike.Solv.Services.WebHook.Domain
{
	[References(typeof(SubscriptionEventLocalIndex))]
	public class Subscription
	{
		/// <summary>
		/// Brand that webhook is associated with
		/// </summary>
		[HashKey]
		public Guid BrandId { get; set; }

		/// <summary>
		/// Primary key
		/// </summary>
		[RangeKey]
		public Guid Id { get; set; }

		/// <summary>
		/// Event that webhook is subscribed for
		/// </summary>
		public int WebHookEvent { get; set; }

		/// <summary>
		/// Owner of the webhook subscription
		/// </summary>
		public Guid? UserId { get; set; }

		/// <summary>
		/// Url to call when event occurs
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Optional secret shared with subscriber, used for generating delivery signature
		/// </summary>
		public string Secret { get; set; }

		/// <summary>
		/// Content for the Authorization header
		/// </summary>
		public string Authorization { get; set; }

		/// <summary>
		/// Http verb type for the notification call
		/// </summary>
		/// <value>POST or PUT</value>
		public string Verb { get; set; }

		/// <summary>
		/// ContentType header
		/// </summary>
		/// <value>application/json</value>
		public string ContentType { get; set; }

		/// <summary>
		/// Template for notification body
		/// </summary>
		public string Body { get; set; }

		/// <summary>
		/// The date the subscription is created.
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The date the subscription is modified.
		/// </summary>
		public DateTime ModifiedDate { get; set; }

		public Subscription(Guid brandId, int eventType, Guid? userId, string url, string body, string verb,
			string contentType, string secret, string authorization)
		{
			BrandId = brandId;
			Id = NewId.NextGuid();
			UserId = userId;
			WebHookEvent = eventType;
			Url = url;
			Body = body;
			Verb = verb;
			ContentType = contentType;
			Secret = secret;
			Authorization = authorization;
			CreatedDate = DateTime.UtcNow;
			ModifiedDate = CreatedDate = DateTime.UtcNow;
		}
	}
}