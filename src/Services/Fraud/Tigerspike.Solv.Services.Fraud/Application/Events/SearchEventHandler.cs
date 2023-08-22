using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Tigerspike.Solv.Services.Fraud.Application.Services;
using Tigerspike.Solv.Services.Fraud.Models;

namespace Tigerspike.Solv.Services.Fraud.Application.Events
{
	public class SearchEventHandler :
		INotificationHandler<TicketDetectionCreatedEvent>,
		INotificationHandler<TicketFraudStatusSetEvent>,
		INotificationHandler<TicketCreatedEvent>
	{
		private readonly ISearchService<FraudSearchModel> _searchService;
		private readonly IFraudService _fraudService;
		private readonly IMapper _mapper;

		public SearchEventHandler(
			ISearchService<FraudSearchModel> searchService,
			IFraudService fraudService,
			IMapper mapper)
		{
			_searchService = searchService;
			_fraudService = fraudService;
			_mapper = mapper;
		}

		public Task Handle(TicketDetectionCreatedEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);
		public async Task Handle(TicketFraudStatusSetEvent notification, CancellationToken cancellationToken)
		{
			var indexTicketTasks = new List<Task>();
			foreach (var id in notification.TicketIds)
			{
				indexTicketTasks.Add(GetAndIndexTicket(id));
			}

			await Task.WhenAll(indexTicketTasks);
		}
		public Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken) => GetAndIndexTicket(notification.TicketId);

		/// <summary>
		/// Fetch the ticket from db and index it.
		/// </summary>
		private async Task GetAndIndexTicket(Guid ticketId)
		{
			var model = _fraudService.GetTicketDetails(ticketId);
			await _searchService.Index(_mapper.Map<FraudSearchModel>(model));
		}
	}
}