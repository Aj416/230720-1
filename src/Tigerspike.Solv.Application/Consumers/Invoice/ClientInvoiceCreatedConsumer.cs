using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Domain.Events;

namespace Tigerspike.Solv.Application.Consumers.Invoice
{
	public class ClientInvoiceCreatedConsumer : IConsumer<ClientInvoiceCreatedEvent>
	{
		private readonly ILogger<ClientInvoiceCreatedConsumer> _logger;

		public ClientInvoiceCreatedConsumer(
			ILogger<ClientInvoiceCreatedConsumer> logger
		)
		{
			_logger = logger;
		}

		public Task Consume(ConsumeContext<ClientInvoiceCreatedEvent> context)
		{
			_logger.LogDebug("Invoice for client {brandId} created", context.Message.BrandId);
			return Task.CompletedTask;
		}
	}
}