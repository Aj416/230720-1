using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Messaging.Invoicing;
using Tigerspike.Solv.Services.Invoicing.Application.Commands;
using Tigerspike.Solv.Services.Invoicing.Application.IntegrationEvents;

namespace Tigerspike.Solv.Services.Invoicing.Application.EventHandlers
{
	public class InvoiceEventHandler :
		INotificationHandler<BillingDetailsCreatedEvent>,
		INotificationHandler<PaymentCreatedEvent>
	{
		private readonly ILogger<InvoiceEventHandler> _logger;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;

		public InvoiceEventHandler(
			ILogger<InvoiceEventHandler> logger,
			IBus bus,
			IOptions<BusOptions> busOptions)
		{
			_logger = logger;
			_bus = bus;
			_busOptions = busOptions.Value;
		}

		public async Task Handle(BillingDetailsCreatedEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogDebug($"Created billing details for brand with Id {notification.BrandId}");
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Brand}"));
			await endpoint.Send<ISetBillingDetailsIdCommand>(new SetBillingDetailsIdCommand(notification.BrandId, notification.BillingDetailsId), cancellationToken);
		}

		public async Task Handle(PaymentCreatedEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogDebug($"Payment made for advocae with Id {notification.AdvocateId}");
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Ticket}"));

			await endpoint.Send<IUpdateAdvocateStatisticsWebhookCommand>(new
			{
				notification.AdvocateId
			}, cancellationToken);
		}
	}
}
