using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Invoice;
using Tigerspike.Solv.Domain.Events.Invoicing;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.CommandHandlers
{
	public class InvoiceCommandHandler : CommandHandler,
		IRequestHandler<PayAdvocateInvoiceCommand, Unit>,
		IRequestHandler<PayClientInvoiceCommand, Unit>,
		IRequestHandler<PayAdvocateInvoiceLineItemCommand, Unit>,
		IRequestHandler<CreateBillingDetailsCommand, Unit>
	{
		private readonly InvoicingOptions _invoicingOptions;
		private readonly IClientInvoiceRepository _clientInvoiceRepository;
		private readonly IAdvocateRepository _advocateRepository;
		private readonly IAdvocateInvoiceRepository _advocateInvoiceRepository;
		private readonly IBrandRepository _brandRepository;
		private readonly ITicketRepository _ticketRepository;
		private readonly IPaymentService _paymentService;
		private readonly ITimestampService _timestampService;
		private readonly IBillingDetailsRepository _billingDetailsRepository;
		private readonly ILogger<InvoiceCommandHandler> _logger;
		private readonly IPaymentRepository _paymentRepository;

		public InvoiceCommandHandler(
			IOptions<InvoicingOptions> invoicingOptions,
			IClientInvoiceRepository clientInvoiceRepository,
			IAdvocateRepository advocateRepository,
			IAdvocateInvoiceRepository advocateInvoiceRepository,
			IBrandRepository brandRepository,
			ITicketRepository ticketRepository,
			IBillingDetailsRepository billingDetailsRepository,
			IPaymentRepository paymentRepository,
			IPaymentService paymentService,
			ITimestampService timestampService,
			ILogger<InvoiceCommandHandler> logger,
			IUnitOfWork uow,
			IMediatorHandler mediator,
			IDomainNotificationHandler notifications) : base(uow, mediator, notifications)
		{
			_invoicingOptions = invoicingOptions?.Value ??
				throw new ArgumentNullException(nameof(invoicingOptions));
			_brandRepository = brandRepository ??
				throw new ArgumentNullException(nameof(brandRepository));
			_ticketRepository = ticketRepository ??
				throw new ArgumentNullException(nameof(ticketRepository));
			_clientInvoiceRepository = clientInvoiceRepository ??
				throw new ArgumentNullException(nameof(clientInvoiceRepository));
			_advocateRepository = advocateRepository ??
				throw new ArgumentNullException(nameof(advocateRepository));
			_advocateInvoiceRepository = advocateInvoiceRepository ??
				throw new ArgumentNullException(nameof(advocateInvoiceRepository));
			_paymentService = paymentService ??
				throw new ArgumentNullException(nameof(paymentService));
			_timestampService = timestampService ??
					throw new ArgumentNullException(nameof(timestampService));
			_logger = logger;
			_billingDetailsRepository = billingDetailsRepository;
			_paymentRepository = paymentRepository ??
					throw new ArgumentNullException(nameof(paymentRepository));
		}

		public async Task<Unit> Handle(PayAdvocateInvoiceCommand request, CancellationToken cancellationToken)
		{
			// Get the advocate invoice
			var invoice = await _advocateInvoiceRepository.GetFirstOrDefaultAsync(
				predicate: inv => inv.Id == request.AdvocateInvoiceId,
				include: inc => inc
					.Include(ii => ii.InvoicingCycle)
					.Include(ii => ii.LineItems).ThenInclude(ti => ti.Brand)
					.Include(ii => ii.Advocate).ThenInclude(ti => ti.User)
			);

			// Get the list of the tickets information to be listed in the PayPal order.
			var invoicedTickets = await _ticketRepository.GetTicketsInfoForAdvocateInvoice(invoice.Id);

			// common for all the payments
			var advocatePaymentAccountId = invoice.Advocate.PaymentAccountId;
			var currencyCode = _invoicingOptions.CurrencyCode;
			var fullName = $"{invoice.Advocate.User.FirstName} {invoice.Advocate.User.LastName}";
			var from = invoice.InvoicingCycle.From.ToString("dd/MM/yyyy");
			var to = invoice.InvoicingCycle.To.Date.AddDays(-1).ToString("dd/MM/yyyy");
			var itemsToPay = invoice.LineItems.Where(x => x.Amount > 0).ToList();

			// The payment is against a line item of the advocate invoice (since each one represent a payment from a brand).
			foreach (var itemToPay in itemsToPay)
			{
				var description = $"Payment from { itemToPay.Brand.Name } to { fullName } for the period: {from} to {to}";
				var breakdown = invoicedTickets
					.Where(x => x.brandId == itemToPay.BrandId)
					.Select(x => ($"Ticket {x.id}", x.price))
					.ToList();

				var payCommand = new PayAdvocateInvoiceLineItemCommand()
				{
					AdvocateInvoiceLineItemId = itemToPay.Id,
					BrandId = itemToPay.BrandId,
					AdvocateId = invoice.AdvocateId,
					AdvocateInvoiceId = invoice.Id,
					BrandPaymentAccountId = itemToPay.Brand.PaymentAccountId,
					SolverPaymentAccountId = advocatePaymentAccountId,
					BillingAgreementId = itemToPay.Brand.BillingAgreementId,
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

		public async Task<Unit> Handle(PayAdvocateInvoiceLineItemCommand request, CancellationToken cancellationToken)
		{
			var requestId = request.AdvocateInvoiceLineItemId.ToString();
			var trackingId = await _paymentService.SetRiskTransactionContext(await GetRiskTransactionContext(request.BrandId, request.AdvocateId));
			var invoice = await _advocateInvoiceRepository.FindAsync(request.AdvocateInvoiceId);
			string referenceId;
			try
			{
				referenceId = await _paymentService.ExecutePayment(requestId, trackingId, request.BrandPaymentAccountId, request.BillingAgreementId, request.Amount, request.CurrencyCode, request.SolverPaymentAccountId, requestId, request.Description, request.Breakdown);
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

			await _paymentRepository.InsertAsync(payment);
			await _paymentRepository.SaveChangesAsync();

			// Update the advocate invoice with the failed date of the payment
			invoice.SetPaidAmount(payment.Amount);
			_advocateInvoiceRepository.Update(invoice);
			await _advocateInvoiceRepository.SaveChangesAsync();

			await _mediator.RaiseEvent(new PaymentCreatedEvent(payment.Id, invoice.AdvocateId, null));

			return Unit.Value;
		}

		public async Task<Unit> Handle(PayClientInvoiceCommand request, CancellationToken cancellationToken)
		{
			// Get the client invoice
			var invoice = await _clientInvoiceRepository.GetFirstOrDefaultAsync(
				predicate: inv => inv.Id == request.ClientInvoiceId,
				include: inc => inc
					.Include(ii => ii.InvoicingCycle)
					.Include(ii => ii.Brand)
			);

			var platformPaymentAccountId = _paymentService.PaymentReceiverAccountId;
			var currencyCode = _invoicingOptions.CurrencyCode;

			var requestId = invoice.Id.ToString(); // to make payment idempotent, "The server stores keys for 3-72 hours depending on account settings"
			var amount = invoice.InvoiceTotal;
			var items = new[] {
 				("Calculated fees", invoice.Fee),
 			}.ToList();

			if (invoice.VatAmount != null)
			{
				items.Add(("VAT", invoice.VatAmount.Value));
			}

			var trackingId = await _paymentService.SetRiskTransactionContext(await GetRiskTransactionContext(invoice.BrandId, platformPaymentAccountId));
			string referenceId;
			try
			{
				referenceId = await _paymentService.ExecutePayment(requestId, trackingId, invoice.Brand.PaymentAccountId, invoice.Brand.BillingAgreementId, amount, currencyCode, platformPaymentAccountId, invoice.Id.ToString(), string.Empty, items);
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

			await _paymentRepository.InsertAsync(payment);
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

		private async Task<RiskTransactionContext> GetRiskTransactionContext(Guid brandId, string platformPaymentAccountId)
		{
			var platformDetailsId = await _billingDetailsRepository.GetCurrentIdForPlatform();
			var platformDetails = await _billingDetailsRepository.FindAsync(platformDetailsId);
			var firstPaymentDate = await _paymentRepository.GetFirstDate(brandId);

			return await GetRiskTransactionContext(brandId,
				Tigerspike.Solv.Domain.Models.Brand.PracticeBrandId,
				platformDetails.CreatedDate,
				platformDetails.Email,
				5.0m,
				platformPaymentAccountId,
				firstPaymentDate
			);
		}

		private async Task<RiskTransactionContext> GetRiskTransactionContext(Guid brandId, Guid advocateId)
		{
			var advocateDetails = await _advocateRepository.GetSingleOrDefaultAsync(
				selector: x => new
				{
					x.User.CreatedDate,
					x.User.Email,
					x.Brands.Single(b => b.BrandId == brandId).Csat,
					x.PaymentAccountId
				},
				predicate: x => x.Id == advocateId
			);
			var firstPaymentDate = await _paymentRepository.GetFirstDate(brandId, advocateId);

			return await GetRiskTransactionContext(brandId,
				advocateId,
				advocateDetails.CreatedDate,
				advocateDetails.Email,
				advocateDetails.Csat,
				advocateDetails.PaymentAccountId,
				firstPaymentDate
			);
		}

		private async Task<RiskTransactionContext> GetRiskTransactionContext(Guid brandId, Guid receiverId, DateTime receiverCreatedDate, string receiverEmail, decimal receiverCsat, string receiverPaymentAccountId, DateTime? firstPaymentDate)
		{
			var brandCreatedDate = await _brandRepository.GetSingleOrDefaultAsync(x => x.CreatedDate, x => x.Id == brandId);
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

		public async Task<Unit> Handle(CreateBillingDetailsCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.GetFirstOrDefaultAsync(
				predicate: x => x.Id == request.BrandId
			);

			if (brand != null)
			{
				var billingDetails = new BillingDetails(request.Name, request.Email, request.VatNumber, request.CompanyNumber, request.Address, request.IsPlatformOwner);
				await _billingDetailsRepository.InsertAsync(billingDetails);

				brand.AssignBillingDetailsId(billingDetails.Id);
				_brandRepository.Update(brand);

				if (await Commit())
				{
					// raise event if you like
				}
				else
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to create new billing details for brand {request.BrandId} {brand.Name} / {request.Name}"));
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Brand {request.BrandId} is not found"));
			}

			return Unit.Value;
		}
	}
}