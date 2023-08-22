using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Services.Fraud.Application.Events;
using Tigerspike.Solv.Services.Fraud.Enum;
using Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.Fraud.Application.Commands.Ticket
{
	public class TicketCommandHandler :
		IRequestHandler<CreateTicketCommand, Unit>,
		IRequestHandler<BulkSetTicketFraudStatusCommand, Unit>
	{
		private readonly ITicketRepository _ticketRepository;
		private readonly IMediatorHandler _mediator;
		private readonly ILogger<TicketCommandHandler> _logger;

		public TicketCommandHandler(
			ITicketRepository ticketRepository,
			IMediatorHandler mediator,
			ILogger<TicketCommandHandler> logger)
		{
			_ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		private Task SetFraudStatus(Domain.Ticket ticket, FraudStatus fraudStatus)
		{
			if (ticket != null)
			{
				ticket.SetFraudResult(fraudStatus);
			}

			return Task.CompletedTask;
		}

		public async Task<Unit> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
		{
			var ticket = _ticketRepository.GetTicket(request.TicketId.ToString());

			if (ticket == null)
			{
				ticket = new Domain.Ticket(request.TicketId, request.CustomerId, request.BrandId, request.BrandName, request.Level, request.Status,
					request.AdvocateName, request.Metadata, new Domain.Customer(request.CustomerFirstName, request.CustomerLastName,
					request.CustomerEmail, request.IpAddress), request.Question);

				_ticketRepository.AddOrUpdateTicket(ticket);
			}
			else
			{
				ticket.UpdateTicket(request.AdvocateName, request.Level, request.Status);
				_ticketRepository.AddOrUpdateTicket(ticket);
			}

			await _mediator.RaiseEvent(new TicketCreatedEvent(request.TicketId));
			return Unit.Value;
		}

		public async Task<Unit> Handle(BulkSetTicketFraudStatusCommand request, CancellationToken cancellationToken)
		{
			var getTicketTasks = new List<Task<Domain.Ticket>>();
			foreach (var item in request.Items)
			{
				getTicketTasks.Add(Task.Run(() => _ticketRepository.GetTicket(item.ToString())));
			}

			var tickets = await Task.WhenAll(getTicketTasks);

			var setStatusTasks = new List<Task>();
			foreach (var ticket in tickets)
			{
				setStatusTasks.Add(SetFraudStatus(ticket, request.FraudStatus));
			}

			await Task.WhenAll(setStatusTasks);

			_ticketRepository.BulkUpdateTicket(tickets);

			if (tickets.Count() > 0)
			{
				await _mediator.RaiseEvent(new TicketFraudStatusSetEvent(request.Items));
			}

			return Unit.Value;
		}
	}
}
