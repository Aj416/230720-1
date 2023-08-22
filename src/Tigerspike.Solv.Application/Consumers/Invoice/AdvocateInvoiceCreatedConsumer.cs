using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Domain.Events;

namespace Tigerspike.Solv.Application.Consumers.Invoice
{
	public class AdvocateInvoiceCreatedConsumer : IConsumer<AdvocateInvoiceCreatedEvent>
	{

		private readonly ILogger<AdvocateInvoiceCreatedConsumer> _logger;

		public AdvocateInvoiceCreatedConsumer(
			ILogger<AdvocateInvoiceCreatedConsumer> logger
		)
		{
			_logger = logger;
		}

		public Task Consume(ConsumeContext<AdvocateInvoiceCreatedEvent> context)
		{
			_logger.LogDebug("Invoice for advocate {advocateId} created", context.Message.AdvocateId);
			return Task.CompletedTask;
		}
	}
}