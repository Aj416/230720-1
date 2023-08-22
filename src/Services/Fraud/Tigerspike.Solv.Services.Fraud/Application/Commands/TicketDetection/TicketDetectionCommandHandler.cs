using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Services.Fraud.Application.Events;
using Tigerspike.Solv.Services.Fraud.Enum;
using Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.Fraud.Application.Commands.TicketDetection
{
	public class TicketDetectionCommandHandler :
		IRequestHandler<CreateTicketDetectionCommand, Unit>
	{
		private readonly ITicketDetectionRepository _ticketDetectionRepository;
		private readonly ITicketRepository _ticketRepository;
		private readonly IRuleRepository _ruleRepository;
		private readonly IMediatorHandler _mediator;
		private readonly IMapper _mapper;

		public TicketDetectionCommandHandler(
			ITicketDetectionRepository ticketDetectionRepository,
			ITicketRepository ticketRepository,
			IRuleRepository ruleRepository,
			IMediatorHandler mediator,
			IMapper mapper)
		{
			_ticketDetectionRepository = ticketDetectionRepository ??
										 throw new ArgumentNullException(nameof(ticketDetectionRepository));
			_ticketRepository = ticketRepository;
			_ruleRepository = ruleRepository;
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			_mapper = mapper;
		}

		public async Task<Unit> Handle(CreateTicketDetectionCommand request, CancellationToken cancellationToken)
		{
			var ticketDetection = _ticketDetectionRepository.GetTicketDetection(request.TicketId.ToString());
			if (ticketDetection == null)
			{
				ticketDetection = new Domain.TicketDetection(request.TicketId, request.Status, request.Rules.ToList());

				_ticketDetectionRepository.AddOrUpdateDetection(ticketDetection);
			}
			else
			{
				ticketDetection.Status = (TicketStatus)request.Status;
				ticketDetection.Rules.AddRange(request.Rules.Select(r => r.ToString()).Except(ticketDetection.Rules));
				_ticketDetectionRepository.AddOrUpdateDetection(ticketDetection);
			}

			if (ticketDetection.Rules.Any())
			{
				var ticket = _ticketRepository.GetTicket(request.TicketId.ToString());

				var lowRiskRules = _ruleRepository.GetByRiskLevel(RiskLevel.Low);
				var otherRisks = ticketDetection.Rules.Except(lowRiskRules).ToList();

				if (ticket.FraudStatus == FraudStatus.None && otherRisks.Count > 0)
				{
					ticket.SetFraudResult(FraudStatus.FraudSuspected);
				}

				_ticketRepository.AddOrUpdateTicket(ticket);
			}

			await _mediator.RaiseEvent(new TicketDetectionCreatedEvent(request.TicketId));

			return Unit.Value;
		}
	}
}
