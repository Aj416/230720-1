using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IUpdateAdvocateStatisticsWebhookCommand
	{
		Guid? AdvocateId { get; set; }
	}
}
