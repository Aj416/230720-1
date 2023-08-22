using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	public class TicketProbingFormEventHandler :
		INotificationHandler<TicketCreatedEvent>
	{
		private readonly IMediatorHandler _mediator;
		private readonly ITicketService _ticketService;
		private readonly ITicketRepository _ticketRepository;

		public TicketProbingFormEventHandler(
			IMediatorHandler mediator,
			ITicketService ticketService,
			ITicketRepository ticketRepository)
		{
			_mediator = mediator;
			_ticketService = ticketService;
			_ticketRepository = ticketRepository;
		}

		public async Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
		{
			var probingResults = await _ticketRepository.GetProbingResults(notification.TicketId);
			var flow = _ticketService.GetProbingEvaluation(probingResults);

			var cmd = flow.action switch
			{
				TicketFlowAction.Escalate => new EscalateTicketCommand(notification.TicketId, TicketEscalationReason.ProbingForm),
				TicketFlowAction.PushbackToClient => new EscalateTicketCommand(notification.TicketId, TicketEscalationReason.ProbingForm, TicketLevel.PushedBack),
				_ => null,
			};

			if (cmd != null)
			{
				await _mediator.SendCommand(cmd);
			}

		}

	}
}