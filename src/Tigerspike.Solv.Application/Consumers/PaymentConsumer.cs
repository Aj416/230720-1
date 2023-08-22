using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Application.Consumers
{
	public class PaymentConsumer :
		IConsumer<IRiskTransactionContextCommand>,
		IConsumer<IExecutePaymentCommand>,
		IConsumer<IFetchPaymentReceiverAccountIdCommand>
	{
		private readonly IPaymentService _paymentService;
		private readonly ILogger<PaymentConsumer> _logger;

		public PaymentConsumer(
			IPaymentService paymentService,
			ILogger<PaymentConsumer> logger)
		{
			_paymentService = paymentService;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<IRiskTransactionContextCommand> context)
		{
			var msg = context.Message;
			_logger.LogDebug("Getting risk transaction context");

			var trackingId = await _paymentService.SetRiskTransactionContext(new RiskTransactionContext
			{
				TrackingId = msg.TrackingId,
				SenderAccountId = msg.SenderAccountId,
				SenderCreatedDate = msg.SenderCreatedDate,
				ReceiverAccountId = msg.ReceiverAccountId,
				ReceiverCreatedDate = msg.ReceiverCreatedDate,
				ReceiverPayPalAccountId = msg.ReceiverPayPalAccountId,
				ReceiverEmail = msg.ReceiverEmail,
				ReceiverAddressCountryCode = msg.ReceiverAddressCountryCode,
				ReceiverPopularityScore = msg.ReceiverPopularityScore,
				FirstInteractionDate = msg.FirstInteractionDate,
				TxnCountTotal = msg.TxnCountTotal
			});

			await context.RespondAsync<IRiskTransactionContextResult>(new
			{
				IsSuccess = true,
				TrackingId = trackingId
			});
		}

		public async Task Consume(ConsumeContext<IExecutePaymentCommand> context)
		{
			var msg = context.Message;
			_logger.LogDebug("Executing payment");

			var referenceId = await _paymentService.ExecutePayment(msg.RequestId, msg.TrackingId, msg.PaymentAccountId, msg.BillingAgreementId, msg.Amount, msg.CurrencyCode, msg.PlatformPaymentAccountId, msg.InvoiceId, msg.Description, msg.Items);

			await context.RespondAsync<IExecutePaymentResult>(new
			{
				IsSuccess = true,
				ReferenceId = referenceId
			});
		}

		public async Task Consume(ConsumeContext<IFetchPaymentReceiverAccountIdCommand> context)
		{
			_logger.LogDebug("Fetch Payment Receiver AccountId");

			await context.RespondAsync<IFetchPaymentReceiverAccountIdResult>(new
			{
				IsSuccess = true,
				PlatformPaymentAccountId = _paymentService.PaymentReceiverAccountId
			});
		}
	}
}
