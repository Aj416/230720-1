using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Search.Interfaces;

namespace Tigerspike.Solv.Search.CommandHandlers
{
	/// <summary>
	/// The command handler class for Ticket Search
	/// </summary>
	///
	public class TicketSearchCommandHandler : CommandHandler,
		IRequestHandler<UpdateTicketIndexCommand, Unit>
	{
		#region Private Properties

		/// <summary>
		/// The auto mapper
		/// </summary>
		private readonly IMapper _mapper;

		/// <summary>
		/// The search service for ticket
		/// </summary>
		private readonly ISearchService<TicketSearchModel> _searchService;

		/// <summary>
		/// The ticket repository
		/// </summary>
		private readonly ITicketRepository _ticketRepository;
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="mapper"></param>
		/// <param name="searchService"></param>
		/// <param name="ticketRepository"></param>
		/// <param name="unitOfWork"></param>
		/// <param name="mediator"></param>
		/// <param name="notificationHander"></param>
		public TicketSearchCommandHandler(IMapper mapper, ISearchService<TicketSearchModel> searchService, ITicketRepository ticketRepository,
			IUnitOfWork unitOfWork, IMediatorHandler mediator, IDomainNotificationHandler notificationHander) : base(unitOfWork, mediator,notificationHander)
		{
			_mapper = mapper;
			_searchService = searchService;
			_ticketRepository = ticketRepository;
		}
		#endregion

		#region UpdateTicketIndexCommand Handler

		/// <summary>
		/// The handler for UpdateTicketIndexCommand
		/// </summary>
		/// <param name="request"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Unit> Handle(UpdateTicketIndexCommand request, CancellationToken cancellationToken)
		{
			var model = await _ticketRepository.GetFirstOrDefaultAsync<TicketSearchModel>(_mapper, x => x.Id == request.TicketId);
			if (model?.IsPractice == false)
			{
				await _searchService.Index(model);
			}

			return Unit.Value;
		}
		#endregion
	}
}
