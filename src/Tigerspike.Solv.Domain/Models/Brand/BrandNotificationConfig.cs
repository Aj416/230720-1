using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Models
{
	public class BrandNotificationConfig
	{
		public Guid Id { get; }
		public Guid BrandId { get; }
		public Brand Brand { get; }
		public bool IsActive { get; }
		public BrandNotificationType Type { get; }
		public int DeliverAfterSeconds { get; }
		public string Subject { get; }
		public string Header { get; }
		public string Body { get; }

		public BrandNotificationConfig(Guid brandId, bool isActive, BrandNotificationType type, int deliverAfterSeconds, string subject, string header, string body)
		{
			BrandId = brandId;
			IsActive = isActive;
			Type = type;
			DeliverAfterSeconds = deliverAfterSeconds;
			Subject = subject;
			Header = header;
			Body = body;
		}
	}
}