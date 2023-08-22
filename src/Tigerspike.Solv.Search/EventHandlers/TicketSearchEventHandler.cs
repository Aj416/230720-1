using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Search.Interfaces;

namespace Tigerspike.Solv.Search.EventHandlers
{
	public class TicketSearchEventHandler :
		INotificationHandler<TicketReservedEvent>,
		INotificationHandler<TicketRejectedEvent>,
		INotificationHandler<TicketAcceptedEvent>,
		INotificationHandler<TicketAbandonedEvent>,
		INotificationHandler<TicketSolvedEvent>,
		INotificationHandler<TicketReopenedEvent>,
		INotificationHandler<TicketClosedEvent>,
		INotificationHandler<TicketEscalatedEvent>,
		INotificationHandler<TicketComplexitySetEvent>,
		INotificationHandler<TicketCsatSetEvent>,
		INotificationHandler<TicketPriceChangedEvent>,
		INotificationHandler<TicketDeletedEvent>,
		INotificationHandler<TicketImportedEvent>
	{
		private readonly ISearchService<TicketSearchModel> _searchService;
		private readonly ITicketRepository _ticketRepository;
		private readonly IMapper _mapper;

		public TicketSearchEventHandler(
			ISearchService<TicketSearchModel> searchService,
			ITicketRepository ticketRepository,
			IMapper mapper)
		{
			_searchService = searchService;
			_ticketRepository = ticketRepository;
			_mapper = mapper;
		}

		public Task Handle(TicketComplexitySetEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);
		public Task Handle(TicketCsatSetEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);
		public Task Handle(TicketImportedEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);
		public Task Handle(TicketPriceChangedEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);

		public async Task Handle(TicketDeletedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.IsPractice)
			{
				// We don't index practice ticket in the first place.
				return;
			}
			await _searchService.Delete(notification.TicketId);
		}

		public Task Handle(TicketReservedEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);

		public Task Handle(TicketRejectedEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);

		public Task Handle(TicketAcceptedEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);

		public Task Handle(TicketAbandonedEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);

		public Task Handle(TicketSolvedEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);

		public Task Handle(TicketReopenedEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);

		public Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);

		public Task Handle(TicketEscalatedEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);

		private async Task GetAndIndexTicket(Guid ticketId)
		{
			var model = await _ticketRepository.GetFirstOrDefaultAsync<TicketSearchModel>(_mapper, x => x.Id == ticketId);
			if (model?.IsPractice == false)
			{
				await _searchService.Index(model);
			}
		}
	}
}
