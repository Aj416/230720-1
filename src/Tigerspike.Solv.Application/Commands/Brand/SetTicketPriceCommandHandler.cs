using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Brand;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.CommandHandlers
{
	public class SetTicketPriceCommandHandler : CommandHandler, IRequestHandler<SetTicketPriceCommand, Unit>
	{
		private readonly IBrandRepository _brandRepository;
		private readonly IBrandTicketPriceHistoryRepository _brandTicketPriceHistoryRepository;

		public SetTicketPriceCommandHandler(
			IBrandRepository brandRepository,
			IBrandTicketPriceHistoryRepository brandTicketPriceHistoryRepository,
			IUnitOfWork uow,
			IMediatorHandler mediator,
			IDomainNotificationHandler notifications) : base(uow, mediator, notifications)
		{
			_brandRepository = brandRepository;
			_brandTicketPriceHistoryRepository = brandTicketPriceHistoryRepository;
		}

		public async Task<Unit> Handle(SetTicketPriceCommand request, CancellationToken cancellationToken)
		{
			// update brand price values
			var brand = await _brandRepository.FindAsync(request.BrandId);
			brand.SetTicketPrice(request.Price);
			_brandRepository.Update(brand);

			// store history entry
			var historyEntry = new BrandTicketPriceHistory(brand.Id, request.Price, request.UserId);
			_brandTicketPriceHistoryRepository.Insert(historyEntry);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new TicketPriceSetEvent(brand.Id, brand.TicketPrice, brand.FeePercentage));
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(nameof(SetTicketPriceCommand), "Cannot set new ticket price for the brand"));
			}

			return Unit.Value;
		}
	}
}