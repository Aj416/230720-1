using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Brand;
using Tigerspike.Solv.Domain.Events;

namespace Tigerspike.Solv.Application.EventHandlers
{
	public class TicketPriceSetEventHandler : INotificationHandler<TicketPriceSetEvent>
	{
		private readonly IMediatorHandler _mediator;
        private readonly IBrandService _brandService;

		public TicketPriceSetEventHandler(
			IBrandService brandService,
			IMediatorHandler mediator)
		{
            _brandService = brandService;
			_mediator = mediator;
		}

		public async Task Handle(TicketPriceSetEvent notification, CancellationToken cancellationToken)
		{
			var fee = _brandService.CalculateTicketFee(notification.Price, notification.FeePercentage);
			await _mediator.SendCommand(new SetPricingForNewTicketsCommand(notification.BrandId, notification.Price, fee));
		}
	}
}
