using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Messaging.Invoicing;
using Tigerspike.Solv.Services.Invoicing.Application.Commands;
using Tigerspike.Solv.Services.Invoicing.Application.IntegrationEvents;
using Tigerspike.Solv.Services.Invoicing.Domain;
using Tigerspike.Solv.Services.Invoicing.Infrastructure.Interfaces;
using Tigerspike.Solv.Services.Invoicing.Models;

namespace Tigerspike.Solv.Services.Invoicing.Application.CommandHandlers
{
	public class InvoicePaymentCommandHandler : CommandHandler,
		IRequestHandler<PayAdvocateInvoiceCommand, Unit>,
		IRequestHandler<PayAdvocateInvoiceLineItemCommand, Unit>,
		IRequestHandler<PayClientInvoiceCommand, Unit>
	{
		private readonly InvoicingOptions _invoicingOptions;
		private readonly IAdvocateInvoiceRepository _advocateInvoiceRepository;
		private readonly IClientInvoiceRepository _clientInvoiceRepository;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IBillingDetailsRepository _billingDetailsRepository;
		private readonly ILogger<InvoicePaymentCommandHandler> _logger;
		private readonly ITimestampService _timestampService;
		private readonly IRequestClient<IFetchTicketsInfoForAdvocateInvoiceCommand> _ticketClient;
		private readonly IRequestClient<IFetchAdvocateInfoCommand> _advocateClient;
		private readonly IRequestClient<IBrandInfoCommand> _brandClient;
		private readonly IRequestClient<IFetchAdvocateDetailsCommand> _advocateBrandClient;
		private readonly IRequestClient<IRiskTransactionContextCommand> _riskTransactionClient;
		private readonly IRequestClient<IExecutePaymentCommand> _paymentClient;
		private readonly IRequestClient<IFetchPaymentReceiverAccountIdCommand> _paypalClient;

		public InvoicePaymentCommandHandler(
			IOptions<InvoicingOptions> invoicingOptions,
			IAdvocateInvoiceRepository advocateInvoiceRepository,
			IClientInvoiceRepository clientInvoiceRepository,
			IPaymentRepository paymentRepository,
			IBillingDetailsRepository billingDetailsRepository,
			ILogger<InvoicePaymentCommandHandler> logger,
			ITimestampService timestampService,
			IRequestClient<IFetchTicketsInfoForAdvocateInvoiceCommand> ticketClient,
			IRequestClient<IFetchAdvocateInfoCommand> advocateClient,
			IRequestClient<IBrandInfoCommand> brandClient,
			IRequestClient<IFetchAdvocateDetailsCommand> advocateBrandClient,
			IRequestClient<IRiskTransactionContextCommand> riskTransactionClient,
			IRequestClient<IExecutePaymentCommand> paymentClient,
			IRequestClient<IFetchPaymentReceiverAccountIdCommand> paypalClient,
			IUnitOfWork uow,
			IMediatorHandler mediator,
			IDomainNotificationHandler notifications) : base(uow, mediator, notifications)
		{
			_invoicingOptions = invoicingOptions?.Value;
			_advocateInvoiceRepository = advocateInvoiceRepository;
			_clientInvoiceRepository = clientInvoiceRepository;
			_paymentRepository = paymentRepository;
			_billingDetailsRepository = billingDetailsRepository;
			_logger = logger;
			_timestampService = timestampService;
			_ticketClient = ticketClient;
			_advocateClient = advocateClient;
			_brandClient = brandClient;
			_advocateBrandClient = advocateBrandClient;
			_riskTransactionClient = riskTransactionClient;
			_paymentClient = paymentClient;
			_paypalClient = paypalClient;
		}

		public async Task<Unit> Handle(PayAdvocateInvoiceCommand request, CancellationToken cancellationToken)
		{
			// Get the advocate invoice
			var invoice = await _advocateInvoiceRepository.GetFirstOrDefaultAsync(
				predicate: inv => inv.Id == request.AdvocateInvoiceId,
				include: inc => inc
					.Include(ii => ii.InvoicingCycle)
					.Include(ii => ii.LineItems)
			);

			if (invoice == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Advocate invoice with Id {request.AdvocateInvoiceId} not found"));
				return Unit.Value;
			}

			try
			{
				var fetchTicketsForAdvocateInvoiceResult =
				await _ticketClient.GetResponse<IFetchTicketsInfoForAdvocateInvoiceResult>(
					new FetchTicketsInfoForAdvocateInvoiceCommand(invoice.Id));

				var fetchAdvocateInfoResult =
				await _advocateClient.GetResponse<IFetchAdvocateInfoResult>(
					new FetchAdvocateInfoCommand(new[] { invoice.AdvocateId }));

				var fetchAdvocateInfoResponse = fetchAdvocateInfoResult.Message.AdvocateInfo.FirstOrDefault();

				// common for all the payments
				var advocatePaymentAccountId = fetchAdvocateInfoResponse.PaymentAccountId;
				var currencyCode = _invoicingOptions.CurrencyCode;
				var fullName = $"{fetchAdvocateInfoResponse.FirstName} {fetchAdvocateInfoResponse.LastName}";
				var from = invoice.InvoicingCycle.From.ToString("dd/MM/yyyy");
				var to = invoice.InvoicingCycle.To.Date.AddDays(-1).ToString("dd/MM/yyyy");
				var itemsToPay = invoice.LineItems.Where(x => x.Amount > 0).ToList();

				// The payment is against a line item of the advocate invoice (since each one represent a payment from a brand).
				foreach (var itemToPay in itemsToPay)
				{
					var brandValidationResult =
						await _brandClient.GetResponse<IBrandValidationResult>(
							new BrandInfoCommand(itemToPay.BrandId));

					var brandValidationResponse = brandValidationResult.Message;
					var description = $"Payment from { brandValidationResponse.Name } to { fullName } for the period: {from} to {to}";
					var breakdown = fetchTicketsForAdvocateInvoiceResult.Message.InvoicedTickets
						.Where(x => x.brandId == itemToPay.BrandId)
						.Select(x => ($"Ticket {x.id}", x.price))
						.ToList();

					var payCommand = new PayAdvocateInvoiceLineItemCommand()
					{
						AdvocateInvoiceLineItemId = itemToPay.Id,
						BrandId = itemToPay.BrandId,
						AdvocateId = invoice.AdvocateId,
						AdvocateInvoiceId = invoice.Id,
						BrandPaymentAccountId = brandValidationResponse.PaymentAccountId,
						SolverPaymentAccountId = advocatePaymentAccountId,
						BillingAgreementId = brandValidationResponse.BillingAgreementId,
						Amount = itemToPay.Amount,
						CurrencyCode = currencyCode,
						Description = description,
						Breakdown = breakdown
					};

					try
					{
						await _mediator.SendCommand(payCommand);
					}
					catch (Exception ex)
					{
						await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Payment execution for advocate invoice line {itemToPay.Id} failed due to the reason: {ex.Message}"));
						_logger.LogError(ex, "An exception occured while trying to execute solver payment for line {advocateInvoiceLineItem}", itemToPay.Id);
					}
				}

				return Unit.Value;
			}
			catch (Exception ex)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Payment execution for advocate invoice {request.AdvocateInvoiceId} failed due to the reason: {ex.Message}"));
				_logger.LogError($"An exception occured while trying to execute solver payment for {request.AdvocateInvoiceId} - {ex}");
				return Unit.Value;
			}
		}

		public async Task<Unit> Handle(PayAdvocateInvoiceLineItemCommand request, CancellationToken cancellationToken)
		{
			var requestId = request.AdvocateInvoiceLineItemId.ToString();

			var riskTransactionContextResult =
				await _riskTransactionClient.GetResponse<IRiskTransactionContextResult>(
					await GetRiskTransactionContext(request.BrandId, request.AdvocateId));

			var invoice = await _advocateInvoiceRepository.FindAsync(request.AdvocateInvoiceId);
			string referenceId;
			try
			{
				var executePaymentResult =
				await _paymentClient.GetResponse<IExecutePaymentResult>(
					new ExecutePaymentCommand(requestId, riskTransactionContextResult.Message.TrackingId, request.BrandPaymentAccountId, request.BillingAgreementId, request.Amount, request.CurrencyCode, request.SolverPaymentAccountId, requestId, request.Description, request.Breakdown));

				referenceId = executePaymentResult.Message.ReferenceId;

				_logger.LogDebug("Payment for advocate invoice line {advocateInvoiceLineItemId} was executed succesfully ({paymentReferenceId})", request.AdvocateInvoiceLineItemId, referenceId);
			}
			catch (Exception ex)
			{
				// Update the advocate invoice with the failed payment date
				invoice.SetFailureDate(_timestampService.GetUtcTimestamp());
				_advocateInvoiceRepository.Update(invoice);
				await _clientInvoiceRepository.SaveChangesAsync();

				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, ex.Message));
				return Unit.Value;
			}

			var payment = new Payment()
			{
				CreatedDate = _timestampService.GetUtcTimestamp(),
				Amount = request.Amount,
				ReferenceNumber = referenceId,
				AdvocateInvoiceLineItemId = request.AdvocateInvoiceLineItemId,
				ClientInvoiceId = null,
			};

			await _paymentRepository.InsertAsync(payment, cancellationToken);
			await _paymentRepository.SaveChangesAsync();

			// Update the advocate invoice with the failed date of the payment
			invoice.SetPaidAmount(payment.Amount);
			_advocateInvoiceRepository.Update(invoice);
			await _advocateInvoiceRepository.SaveChangesAsync();

			await _mediator.RaiseEvent(new PaymentCreatedEvent(invoice.AdvocateId));

			return Unit.Value;
		}

		public async Task<Unit> Handle(PayClientInvoiceCommand request, CancellationToken cancellationToken)
		{
			// Get the client invoice
			var invoice = await _clientInvoiceRepository.GetFirstOrDefaultAsync(
				predicate: inv => inv.Id == request.ClientInvoiceId,
				include: inc => inc
					.Include(ii => ii.InvoicingCycle)
			);

			if (invoice == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Client Invoice with Id {request.ClientInvoiceId} not found."));
				return Unit.Value;
			}

			var currencyCode = _invoicingOptions.CurrencyCode;

			var fetchPaymentReceiverAccountIdResult =
							await _paypalClient.GetResponse<IFetchPaymentReceiverAccountIdResult>(
								new { FetchFromConfiguration = true });

			var requestId = invoice.Id.ToString(); // to make payment idempotent, "The server stores keys for 3-72 hours depending on account settings"
			var amount = invoice.InvoiceTotal;
			var items = new[] {
 				("Calculated fees", invoice.Fee),
 			}.ToList();

			if (invoice.VatAmount != null)
			{
				items.Add(("VAT", invoice.VatAmount.Value));
			}

			var riskTransactionContextResult =
				await _riskTransactionClient.GetResponse<IRiskTransactionContextResult>(
					await GetRiskTransactionContext(invoice.BrandId, fetchPaymentReceiverAccountIdResult.Message.PlatformPaymentAccountId));

			if (riskTransactionContextResult.Message.IsSuccess)
			{
				string referenceId;
				try
				{
					var brandValidationResult = await _brandClient.GetResponse<IBrandValidationResult>(
								new BrandInfoCommand(invoice.BrandId));

					var brandValidationResponse = brandValidationResult.Message;

					var executePaymentResult = await _paymentClient.GetResponse<IExecutePaymentResult>(
					new ExecutePaymentCommand(requestId, riskTransactionContextResult.Message.TrackingId, brandValidationResponse.PaymentAccountId, brandValidationResponse.BillingAgreementId, amount, currencyCode, fetchPaymentReceiverAccountIdResult.Message.PlatformPaymentAccountId, invoice.Id.ToString(), string.Empty, items));

					referenceId = executePaymentResult.Message.IsSuccess ? executePaymentResult.Message.ReferenceId : null;

					_logger.LogDebug("Payment for client invoice {invoiceId} from brand {brandId} to platform was executed succesfully ({paymentReferenceId})", request.ClientInvoiceId, invoice.BrandId, referenceId);
				}
				catch (Exception ex)
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, ex.Message));

					// Update the client invoice with the failed date of the payment
					invoice.SetFailureDate(_timestampService.GetUtcTimestamp());
					_clientInvoiceRepository.Update(invoice);
					await _clientInvoiceRepository.SaveChangesAsync();

					return Unit.Value;
				}

				var payment = new Payment()
				{
					CreatedDate = _timestampService.GetUtcTimestamp(),
					Amount = amount,
					ReferenceNumber = referenceId,
					AdvocateInvoiceLineItemId = null,
					ClientInvoiceId = invoice.Id,
				};

				await _paymentRepository.InsertAsync(payment, cancellationToken);
				invoice.SetPaidAmount(payment.Amount);
				_clientInvoiceRepository.Update(invoice);

				if (await Commit())
				{
					_logger.LogDebug("Payment for client invoice {invoiceId} were executed succesfully", request.ClientInvoiceId);
					return Unit.Value;
				}

				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Executed payment for client invoice {invoice.Id} cannot be saved"));
				return Unit.Value;
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Not able to set the risk transaction context for API call"));
			}

			return Unit.Value;
		}

		private async Task<RiskTransactionContext> GetRiskTransactionContext(Guid brandId, Guid advocateId)
		{
			var fetchAdvocateDetailsResult =
				await _advocateBrandClient.GetResponse<IFetchAdvocateDetailsResult>(
					new FetchAdvocateDetailsCommand(advocateId, brandId));

			if (fetchAdvocateDetailsResult.Message.IsSuccess)
			{
				var fetchAdvocateDetailsResponse = fetchAdvocateDetailsResult.Message;

				var firstPaymentDate = await _paymentRepository.GetFirstDate(brandId, advocateId);

				return await GetRiskTransactionContext(brandId,
				advocateId,
				fetchAdvocateDetailsResponse.CreatedDate,
				fetchAdvocateDetailsResponse.Email,
				fetchAdvocateDetailsResponse.Csat,
				fetchAdvocateDetailsResponse.PaymentAccountId,
				firstPaymentDate);
			}

			return null;
		}

		private async Task<RiskTransactionContext> GetRiskTransactionContext(Guid brandId, Guid receiverId, DateTime receiverCreatedDate, string receiverEmail, decimal receiverCsat, string receiverPaymentAccountId, DateTime? firstPaymentDate)
		{
			var brandValidationResult =
				await _brandClient.GetResponse<IBrandValidationResult>(
					new BrandInfoCommand(brandId));

			if (brandValidationResult.Message.IsSuccess)
			{
				var brandCreatedDate = brandValidationResult.Message.CreatedDate;

				var brandTransactionsCount = await _paymentRepository.CountAsync(x => x.AdvocateInvoiceLineItem.BrandId == brandId);

				return new RiskTransactionContext
				{
					TrackingId = Guid.NewGuid(),
					SenderAccountId = brandId,
					SenderCreatedDate = brandCreatedDate,
					ReceiverAccountId = receiverId,
					ReceiverPayPalAccountId = receiverPaymentAccountId,
					ReceiverCreatedDate = receiverCreatedDate,
					ReceiverEmail = receiverEmail,
					ReceiverPopularityScore = receiverCsat,
					// ReceiverAddressCountryCode = TODO by the 18th Oct 2019 !!
					FirstInteractionDate = firstPaymentDate ?? _timestampService.GetUtcTimestamp(), // if there were no previous transactions, we set this date to "now" as the paypal API will not accept "null"
					TxnCountTotal = brandTransactionsCount,
				};
			}

			return null;
		}

		private async Task<RiskTransactionContext> GetRiskTransactionContext(Guid brandId, string platformPaymentAccountId)
		{
			var platformDetailsId = await _billingDetailsRepository.GetCurrentIdForPlatform();
			var platformDetails = await _billingDetailsRepository.FindAsync(platformDetailsId);
			var firstPaymentDate = await _paymentRepository.GetFirstDate(brandId);

			return await GetRiskTransactionContext(brandId,
				new Guid("11111111-1111-1111-1111-111111111111"),
				platformDetails.CreatedDate,
				platformDetails.Email,
				5.0m,
				platformPaymentAccountId,
				firstPaymentDate
			);
		}
	}
}
