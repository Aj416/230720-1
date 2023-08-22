using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Brand;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	public class TicketRatingResponseHandler :
		INotificationHandler<TicketNpsSetEvent>,
		INotificationHandler<TicketRatingEvent>,
		INotificationHandler<TicketCsatSetEvent>,
		INotificationHandler<TicketAdditionalFeedBackSetEvent>
	{
		private readonly ITicketAutoResponseService _ticketAutoResponseService;
		private readonly ICachedBrandRepository _cachedBrandRepository;

		public TicketRatingResponseHandler(
			ITicketAutoResponseService ticketAutoResponseService,
			ICachedBrandRepository cachedBrandRepository)
		{
			_ticketAutoResponseService = ticketAutoResponseService;
			_cachedBrandRepository = cachedBrandRepository;
		}

		public async Task Handle(TicketRatingEvent notification, CancellationToken cancellationToken)
		{
			if ((notification.TransportType == TicketTransportType.Chat) && notification.AdvocateId != null)
			{
				var brandDetails = await _cachedBrandRepository.GetAsync(notification.BrandId);
				var model = new BrandResponseTemplateModel(brandName: brandDetails.Name);
				await _ticketAutoResponseService.SendResponses(notification.BrandId, notification.TicketId, new[] { BrandAdvocateResponseType.TicketRating }, UserType.Advocate, authorId: notification.AdvocateId, model: model);
			}
		}

		public async Task Handle(TicketNpsSetEvent notification, CancellationToken cancellationToken)
		{
			if (notification.TransportType == TicketTransportType.Chat)
			{
				await _ticketAutoResponseService.SendResponses(notification.BrandId, notification.TicketId, new[] { BrandAdvocateResponseType.TicketNpsRated }, UserType.Advocate, authorId: notification.AdvocateId);
			}
		}

		public async Task Handle(TicketCsatSetEvent notification, CancellationToken cancellationToken)
		{
			if (notification.TransportType == TicketTransportType.Chat)
			{
				var brandDetails = await _cachedBrandRepository.GetAsync(notification.BrandId);
				var model = new BrandResponseTemplateModel(brandName: brandDetails.Name);
				await _ticketAutoResponseService.SendResponses(notification.BrandId, notification.TicketId, new[] { BrandAdvocateResponseType.TicketCsatRated }, UserType.Advocate, authorId: notification.AdvocateId, model: model);
			}
		}

		public async Task Handle(TicketAdditionalFeedBackSetEvent notification, CancellationToken cancellationToken)
		{
			if (notification.TransportType == TicketTransportType.Chat)
			{
				await _ticketAutoResponseService.SendResponses(notification.BrandId, notification.TicketId, new[] { BrandAdvocateResponseType.TicketFeedBacked }, UserType.Advocate);
			}
		}
	}
}