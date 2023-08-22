using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Services.Invoicing.Application.Commands;
using Tigerspike.Solv.Services.Invoicing.Application.IntegrationEvents;
using Tigerspike.Solv.Services.Invoicing.Domain;
using Tigerspike.Solv.Services.Invoicing.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.Invoicing.Application.CommandHandlers
{
	public class InvoiceCommandHandler : CommandHandler,
		IRequestHandler<CreateBillingDetailsCommand, Unit>,
		IRequestHandler<CreateInvoicingCycleCommand, Guid?>
	{
		private readonly IBillingDetailsRepository _billingDetailsRepository;
		private readonly IInvoicingCycleRepository _invoicingCycleRepository;

		public InvoiceCommandHandler(
			IBillingDetailsRepository billingDetailsRepository,
			IInvoicingCycleRepository invoicingCycleRepository,
			IUnitOfWork uow,
			IMediatorHandler mediator,
			IDomainNotificationHandler notifications) : base(uow, mediator, notifications)
		{
			_billingDetailsRepository = billingDetailsRepository;
			_invoicingCycleRepository = invoicingCycleRepository;
		}

		public async Task<Unit> Handle(CreateBillingDetailsCommand request, CancellationToken cancellationToken)
		{
			var billingDetails = new BillingDetails(request.Name, request.Email, request.VatNumber, request.CompanyNumber, request.Address, request.IsPlatformOwner);
			await _billingDetailsRepository.InsertAsync(billingDetails, cancellationToken);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new BillingDetailsCreatedEvent(billingDetails.Id, request.BrandId));
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to create new billing details for brand {request.BrandId} / {request.Name}"));
			}

			return Unit.Value;
		}

		public async Task<Guid?> Handle(CreateInvoicingCycleCommand request, CancellationToken cancellationToken)
		{
			var invoicingCycle = new InvoicingCycle(request.InvoicingCycleStartDate, request.InvoicingCycleEndDate);
			await _invoicingCycleRepository.InsertAsync(invoicingCycle, cancellationToken);

			if (await Commit())
			{
				return invoicingCycle.Id;
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to create new invoicing cycle from date {request.InvoicingCycleStartDate} to {request.InvoicingCycleEndDate}"));
				return null;
			}
		}
	}
}
