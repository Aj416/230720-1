using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.CommandHandlers.Import
{
	public class ImportTicketCommandHandler : CommandHandler,
		IRequestHandler<ImportTicketCommand, Guid?>,
		IRequestHandler<AddTicketImportFailureCommand, Unit>
	{
		private readonly ILogger<ImportTicketCommandHandler> _logger;
		private readonly ITimestampService _timestampService;
		private readonly IUserRepository _userRepository;
		private readonly ITicketImportRepository _ticketImportRepository;
		private readonly ITicketRepository _ticketRepository;
		private readonly ITicketSourceRepository _ticketSourceRepository;
		private readonly IBrandRepository _brandRepository;
		private readonly IBrandService _brandService;

		public ImportTicketCommandHandler(
			IUserRepository userRepository,
			ITicketImportRepository ticketImportRepository,
			ITicketRepository ticketRepository,
			ITicketSourceRepository ticketSourceRepository,
			IBrandRepository brandRepository,
			IBrandService brandService,
			ILogger<ImportTicketCommandHandler> logger,
			ITimestampService timestampService,
			IUnitOfWork uow,
			IMediatorHandler mediator,
			IDomainNotificationHandler notifications) : base(uow, mediator, notifications)
		{
			_logger = logger;
			_timestampService = timestampService;
			_userRepository = userRepository;
			_ticketImportRepository = ticketImportRepository;
			_ticketRepository = ticketRepository;
			_ticketSourceRepository = ticketSourceRepository;
			_brandRepository = brandRepository;
			_brandService = brandService;
		}

		public async Task<Guid?> Handle(ImportTicketCommand request, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Started import command [{referenceId}]", request.ReferenceId);

			var customer = await _userRepository.GetByEmail(request.CustomerEmail) ?? new User(Guid.NewGuid(), request.CustomerFirstName, request.CustomerLastName, request.CustomerEmail, null);
			var source = await _ticketSourceRepository.Get(request.Source);
			var brand = await _brandRepository.FindAsync(request.BrandId);
			var brandTags = await _brandRepository.GetTags(request.BrandId);
			var ticketPrice = request.Price ?? brand.TicketPrice;
			var fee = _brandService.CalculateTicketFee(ticketPrice, brand.FeePercentage);
			var metadata = request.Metadata?.Select(x => new TicketMetadataItem(x.Key, x.Value));
			var ticket = new Ticket(customer, request.Question, brand, ticketPrice, fee, request.ReferenceId, source, request.ImportId, metadata, request.CreatedDate);
			_ticketRepository.Insert(ticket);

			if (request.Tags?.Length > 0)
			{
				var tags = brandTags
					.Where(x => request.Tags.Contains(x.Id))
					.Select(x => x.Id)
					.ToArray();
				ticket.SetTags(tags, ticket.Level, _timestampService.GetUtcTimestamp());
			}

			if (request.AdvocateEmail != null && request.AssignedDate != null)
			{
				var advocate = await _userRepository.GetByEmail(request.AdvocateEmail);
				ticket.Reserve(advocate.Id, request.AssignedDate.Value);
				ticket.Accept(request.AssignedDate.Value);
			}

			if (request.SolvedDate != null)
			{
				ticket.Solve(request.SolvedDate.Value);
			}

			if (request.ClosedDate != null)
			{
				ticket.Close(request.ClosedDate.Value, ClosedBy.System);

				if (request.Complexity != null)
				{
					ticket.SetComplexity(request.Complexity.Value);
				}

				if (request.Csat != null)
				{
					ticket.SetCSAT(request.Csat.Value, request.ClosedDate.Value);
				}
			}

			ticket.SetReady();

			if (await Commit())
			{
				await _mediator.RaiseEvent(new TicketImportedEvent(ticket.Id, ticket.ReferenceId, ticket.BrandId, ticket.AdvocateId, ticket.ClosedDate, ticket.Csat));
				return ticket.Id;
			}

			return null;
		}

		public async Task<Unit> Handle(AddTicketImportFailureCommand request, CancellationToken cancellationToken)
		{
			var import = await _ticketImportRepository.FindAsync(request.ImportId);
			if (import != null)
			{
				import.AddFailure(new TicketImportFailure(request.ImportId, request.RawInput, request.FailureReason));
				_ticketImportRepository.Update(import);
				if (await Commit())
				{
					// event here? if you like
					_logger.LogDebug("Failure result of ticket import saved ({failure})", request.FailureReason);
				}
				else
				{
					_logger.LogError("An error occured while adding failure result of ticket import ({failure})", request.FailureReason);
				}
			}
			else
			{
				_logger.LogError("Cannot add failure result of ticket import, as indicated ImportId ({importId}) does not exists ({failure})", request.ImportId, request.FailureReason);
			}

			return Unit.Value;
		}
	}
}