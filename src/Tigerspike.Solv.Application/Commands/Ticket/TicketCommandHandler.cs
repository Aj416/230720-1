using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Domain.Models;
using Cached = Tigerspike.Solv.Infra.Data.Models.Cached;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.Scheduler;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Messaging.Notification;
using Tigerspike.Solv.Core.Email;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Localization;
using WorkflowCore.Interface;
using Tigerspike.Solv.Application.Constants;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Messaging.Ticket;
using Tigerspike.Solv.Domain.DTOs;

namespace Tigerspike.Solv.Application.CommandHandlers
{
	public class TicketCommandHandler : CommandHandler,
		IRequestHandler<CreateTicketCommand, Guid?>,
		IRequestHandler<DeleteTicketCommand, Unit>,
		IRequestHandler<ReserveTicketCommand, Guid?>,
		IRequestHandler<ReserveAndAcceptTicketCommand, Guid?>,
		IRequestHandler<SetTicketTagsCommand, Unit>,
		IRequestHandler<ReserveEscalatedTicketCommand, Guid?>,
		IRequestHandler<AcceptTicketCommand, Unit>,
		IRequestHandler<SetReturningCustomerStateCommand, Unit>,
		IRequestHandler<SetNotificationResumptionStateCommand, Unit>,
		IRequestHandler<RejectTicketCommand, Unit>,
		IRequestHandler<SolveTicketCommand, Unit>,
		IRequestHandler<AbandonTicketCommand, Unit>,
		IRequestHandler<ReopenTicketCommand, Unit>,
		IRequestHandler<CloseTicketCommand, Unit>,
		IRequestHandler<CompleteTicketCommand, Unit>,
		IRequestHandler<EscalateTicketCommand, Unit>,
		IRequestHandler<ExpireTicketReservationCommand, Ticket>,
		IRequestHandler<SetTicketComplexityCommand, Unit>,
		IRequestHandler<SetTicketCsatCommand, Unit>,
		IRequestHandler<SetTicketSposCommand, Unit>,
		IRequestHandler<SendTicketCreatedEmail>,
		IRequestHandler<SendFirstAdvocateResponseInChatEmailCommand, bool>,
		IRequestHandler<SendTicketClosedEmailCommand, bool>,
		IRequestHandler<ScheduleChatReminderCommand, Unit>,
		IRequestHandler<UpdateTicketMessageStatisticsCommand, Unit>,
		IRequestHandler<SetTicketNpsCommand, Unit>,
		IRequestHandler<SetFraudStatusCommand, Unit>,
		IRequestHandler<IncreaseTicketChaserEmailsCommand, Unit>,
		IRequestHandler<UpdateTicketFraudCommand, Unit>,
		IRequestHandler<SendTicketSposEmail, Unit>,
		IRequestHandler<SetTicketCategoryCommand, Unit>,
		IRequestHandler<SetTicketDiagnosisCommand, bool>,
		IRequestHandler<SetTicketValidTransferCommand, bool>,
		IRequestHandler<SetTicketAdditionalFeedBackCommand, Unit>,
		IRequestHandler<SkipTicketAdditionalFeedBackCommand, Unit>,
		IRequestHandler<SetCustomerRepeatedCountCommand, Unit>
	{
		private readonly ILocalizationProviderFactory _localizationProviderFactory;
		private readonly ITemplateService _templateService;
		private readonly ITimestampService _timestampService;
		private readonly ITicketRepository _ticketRepository;
		private readonly ITicketSourceRepository _ticketSourceRepository;
		private readonly IBrandRepository _brandRepository;
		private readonly ICachedBrandRepository _cachedBrandRepository;
		private readonly IBrandService _brandService;
		private readonly ITicketService _ticketService;
		private readonly ITicketUrlService _ticketUrlService;
		private readonly IAdvocateRepository _advocateRepository;
		private readonly IUserRepository _userRepository;
		private readonly ISchedulerService _schedulerService;
		private readonly IBrandMetadataService _brandMetadataService;
		private readonly IWorkflowController _workflowService;
		private readonly EmailTemplatesOptions _emailTemplateOptions;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;
		private readonly string _ipAddress;
		private readonly string _userAgent;
		private readonly ILogger<TicketCommandHandler> _logger;
		private readonly IChatService _chatService;
		private readonly ITicketTagRepository _ticketTagRepository;

		public TicketCommandHandler(
			IChatService chatService,
			ILocalizationProviderFactory localizationProviderFactory,
			ITemplateService templateService,
			ITimestampService timestampService,
			ITicketRepository ticketRepository,
			ITicketTagRepository ticketTagRepository,
			ITicketSourceRepository ticketSourceRepository,
			IBrandRepository brandRepository,
			ICachedBrandRepository cachedBrandRepository,
			IBrandService brandService,
			ITicketService ticketService,
			ITicketUrlService ticketUrlService,
			IAdvocateRepository advocateRepository,
			IUserRepository userRepository,
			IOptions<EmailTemplatesOptions> emailTemplateOptions,
			IUnitOfWork unitOfWork,
			IMediatorHandler mediator,
			IBus bus,
			IOptions<BusOptions> busOptions,
			ISchedulerService schedulerService,
			IBrandMetadataService brandMetadataService,
			IDomainNotificationHandler notificationHander,
			IWorkflowController workflowService,
			IHttpContextAccessor httpContextAccessor,
			ILogger<TicketCommandHandler> logger) : base(unitOfWork, mediator,
			notificationHander)
		{
			_chatService = chatService;
			_localizationProviderFactory = localizationProviderFactory;
			_templateService = templateService;
			_timestampService = timestampService ??
			                    throw new ArgumentNullException(nameof(timestampService));
			_ticketRepository = ticketRepository ??
			                    throw new ArgumentNullException(nameof(ticketRepository));
			_ticketTagRepository = ticketTagRepository ??
								throw new ArgumentNullException(nameof(ticketTagRepository));
			_ticketSourceRepository = ticketSourceRepository;
			_brandRepository = brandRepository ??
			                   throw new ArgumentNullException(nameof(brandRepository));
			_cachedBrandRepository = cachedBrandRepository;
			_brandService = brandService ??
			                throw new ArgumentNullException(nameof(brandService));
			_ticketService = ticketService;
			_ticketUrlService = ticketUrlService;
			_userRepository = userRepository ??
			                  throw new ArgumentNullException(nameof(ticketRepository));
			_schedulerService = schedulerService;
			_brandMetadataService = brandMetadataService;
			_workflowService = workflowService;
			_emailTemplateOptions = emailTemplateOptions?.Value;
			_advocateRepository = advocateRepository ??
			                      throw new ArgumentNullException(nameof(advocateRepository));
			_bus = bus ??
			       throw new ArgumentNullException(nameof(bus));
			_busOptions = busOptions.Value;
			_ipAddress = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
			var ua = StringValues.Empty;
			if (httpContextAccessor?.HttpContext?.Request?.Headers.TryGetValue("User-Agent", out ua) == true)
			{
				_userAgent = ua.ToString();
			}

			_logger = logger;
		}

		private Task BulkFraudStatusUpdate(Ticket ticket, FraudStatus fraudStatus)
		{
			ticket.SetFraudStatus(fraudStatus);
			return Task.CompletedTask;
		}

		public async Task<Guid?> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
		{
			var ticket = await PrepareTicket(request, _timestampService.GetUtcTimestamp());
			if (ticket != null)
			{
				// Track ticket
				ticket.Track(ticket.Customer, _ipAddress, _userAgent, request.MessageType,
					_timestampService.GetUtcTimestamp());

				await _ticketRepository.InsertAsync(ticket);

				// analyze probing results
				if (ticket.ProbingAnswers?.Count > 0)
				{
					var probingForm = await _brandRepository.GetFirstOrDefaultAsync(
						predicate: x => x.Id == ticket.BrandId,
						selector: x => x.ProbingForm,
						include: x => x
							.Include(inc => inc.ProbingForm)
							.ThenInclude(inc => inc.Questions)
							.ThenInclude(inc => inc.Options)
					);

					var (action, value) = _ticketService.GetProbingEvaluation(ticket.ProbingAnswers, probingForm);
					if (action == TicketFlowAction.Escalate)
					{
						ticket.Escalate(TicketEscalationReason.ProbingForm, null, _timestampService.GetUtcTimestamp());
					}
					else if (action == TicketFlowAction.PushbackToClient)
					{
						ticket.Escalate(TicketEscalationReason.ProbingForm, TicketLevel.PushedBack,
							_timestampService.GetUtcTimestamp());
					}
				}

				if (await Commit())
				{
					await _workflowService.StartWorkflow(WorkflowKeys.CreateTicketKey, WorkflowKeys.CreateTicketVersion
						, new CreateTicketWorkflowModel()
						{
							TicketId = ticket.Id,
							BrandId = ticket.BrandId,
							Culture = ticket.Culture,
							CustomerId = ticket.Customer.Id,
							IsPractice = ticket.IsPractice,
							Level = ticket.Level,
							Question = ticket.Question,
							ReferenceId = ticket.ReferenceId,
							SourceId = ticket.SourceId,
							SourceName = request.Source,
							ThreadId = ticket.ThreadId,
							TransportType = ticket.TransportType,
							Timestamp = DateTime.UtcNow
						});

					return ticket.Id;
				}

				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be created"));
			}

			return null;
		}



		public async Task<Guid?> Handle(ReserveTicketCommand request, CancellationToken cancellationToken)
		{
			// Get the id of the ticket that is already reserved and still valid.
			var ticketId = await _ticketRepository.GetReservedTicketId(request.AdvocateId);

			// If there is a ticket already reserved, simply return it.
			if (ticketId != null)
			{
				return ticketId;
			}

			// Check if the advocate can reserve another ticket.
			var assignedTicketsCount = await _ticketRepository.CountAsync(t =>
				t.AdvocateId == request.AdvocateId && t.Status == TicketStatusEnum.Assigned);
			if (assignedTicketsCount == Advocate.MAX_ASSIGNED_TICKETS)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"Maximum number of assigned tickets has been reached."));
				return null;
			}

			Ticket ticket;

			// Check if the advocate is in practice mode.
			var advocate = await _advocateRepository.GetSingleOrDefaultAsync(
				include: i => i.Include(a => a.Brands),
				predicate: a => a.Id == request.AdvocateId);

			if (advocate.Practicing)
			{
				// If the advocate is in practice, get the ticket that was created for him to practice
				ticket = await _ticketRepository.GetFullTicket(p => p.IsPractice && p.AdvocateId == advocate.Id);
			}
			else
			{
				// Otherwise, reserve a ticket from the pool if any.
				var advocateBrandIds = await _cachedBrandRepository.GetActiveBrandsIds(advocate.Id);

				var reserved =
					await _ticketRepository.ReserveTicket(advocate.Id, advocateBrandIds, TicketLevel.Regular);

				if (reserved)
				{
					ticketId = await _ticketRepository.GetReservedTicketId(advocate.Id);
					ticket = await _ticketRepository.GetFullTicket(p => p.Id == ticketId);
				}
				else
				{
					await _mediator.RaiseEvent(new TicketReservationFailedEvent(advocate.Id, advocateBrandIds.ToArray(),
						TicketLevel.Regular));
					return null;
				}
			}

			if (ticket == null)
			{
				return null;
			}

			var previousAdvocateId = ticket.AdvocateId;
			// Call the reserve method to make the ticket reserved.
			ticket.Reserve(advocate.Id, _timestampService.GetUtcTimestamp());

			if (await Commit())
			{
				// Note - TicketAdvocateChangedEvent should be the first one to be called as it clears cache for the respective ticket.
				await _mediator.RaiseEvent(new TicketAdvocateChangedEvent(ticket.Id, ticket.Status, previousAdvocateId,
					ticket.AdvocateId, ticket.Advocate?.User?.FirstName, ticket.Advocate?.GetCsat(ticket.BrandId)));

				// Raise integration event
				await _bus.Publish<ITicketAdvocateChangedEvent>(
					new
					{
						TicketId = ticket.Id,
						TicketStatus = (int)ticket.Status,
						OldAdvocateId = previousAdvocateId,
						NewAdvocateId = ticket.AdvocateId,
						NewAdvocateFirstName = ticket.Advocate?.User?.FirstName,
						NewAdvocateCsat = ticket.Advocate?.GetCsat(ticket.BrandId)
					}, cancellationToken);

				await _mediator.RaiseEvent(new TicketReservedEvent(ticket.Id, ticket.BrandId, ticket.AdvocateId.Value,
					ticket.Advocate.User.FirstName,
					advocate.Brands.SingleOrDefault(b => b.BrandId == ticket.BrandId).Csat,
					ticket.ReservationExpiryDate.Value, ticket.Customer.Id));

				return ticket.Id;
			}

			return null;
		}

		public async Task<Guid?> Handle(ReserveAndAcceptTicketCommand request, CancellationToken cancellationToken)
		{
			// Check if the advocate can reserve another ticket.
			var assignedTicketsCount = await _ticketRepository.CountAsync(t =>
				t.AdvocateId == request.AdvocateId && t.Status == TicketStatusEnum.Assigned);
			if (assignedTicketsCount == Advocate.MAX_ASSIGNED_TICKETS)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"Maximum number of assigned tickets has been reached."));
				return null;
			}

			Ticket ticket = null;

			// Check if the advocate is in practice mode.
			var advocate = await _advocateRepository.GetSingleOrDefaultAsync(
				include: i => i.Include(a => a.Brands),
				predicate: a => a.Id == request.AdvocateId);

			if (advocate.Practicing)
			{
				// If the advocate is in practice, get the ticket that was created for him to practice
				ticket = await _ticketRepository.GetFullTicket(p => p.IsPractice && p.AdvocateId == advocate.Id);
			}
			else
			{
				// Get the id of the ticket that is already reserved and still valid.
				var ticketId = await _ticketRepository.GetReservedTicketId(request.AdvocateId);

				// If there is a ticket already reserved, simply return it.
				if (ticketId != null)
				{
					ticket = await _ticketRepository.GetFullTicket(t => t.Id == ticketId.Value);
				}
				else
				{
					// Otherwise, reserve a ticket from the pool if any.
					var advocateBrandIds = await _cachedBrandRepository.GetActiveBrandsIds(advocate.Id);
					var reserved =
						await _ticketRepository.ReserveTicket(advocate.Id, advocateBrandIds, TicketLevel.Regular);

					if (reserved)
					{
						ticketId = await _ticketRepository.GetReservedTicketId(advocate.Id);
						ticket = await _ticketRepository.GetFullTicket(t => t.Id == ticketId.Value);
					}
					else
					{
						await _mediator.RaiseEvent(new TicketReservationFailedEvent(advocate.Id,
							advocateBrandIds.ToArray(), TicketLevel.Regular));
					}
				}
			}

			if (ticket == null)
			{
				return null;
			}

			var previousAdvocateId = ticket.AdvocateId;
			ticket.Reserve(advocate.Id, _timestampService.GetUtcTimestamp());

			ticket.Accept(_timestampService.GetUtcTimestamp());

			// Track ticket
			ticket.Track(ticket.Advocate.User, _ipAddress, _userAgent, request.MessageType,
				_timestampService.GetUtcTimestamp());

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				// Note - TicketAdvocateChangedEvent should be the first one to be called as it clears cache for the respective ticket.
				await _mediator.RaiseEvent(new TicketAdvocateChangedEvent(ticket.Id, ticket.Status, previousAdvocateId,
					ticket.AdvocateId, ticket.Advocate?.User?.FirstName, ticket.Advocate?.GetCsat(ticket.BrandId)));

				// Raise integration event
				await _bus.Publish<ITicketAdvocateChangedEvent>(
					new
					{
						TicketId = ticket.Id,
						TicketStatus = (int)ticket.Status,
						OldAdvocateId = previousAdvocateId,
						NewAdvocateId = ticket.AdvocateId,
						NewAdvocateFirstName = ticket.Advocate?.User?.FirstName,
						NewAdvocateCsat = ticket.Advocate?.GetCsat(ticket.BrandId)
					}, cancellationToken);

				await _mediator.RaiseEvent(new TicketAcceptedEvent(ticket.Id, ticket.BrandId, ticket.ReferenceId,
					ticket.IsPractice, ticket.AdvocateId.Value,
					ticket.Advocate.User.FirstName, ticket.Advocate.GetCsat(ticket.BrandId), ticket.Advocate.Super,
					ticket.Source?.Name, ticket.Level, ticket.EscalationReason, ticket.TransportType,
					ticket.FirstAssignedDate.Value, ticket.AssignedDate.Value, ticket.Customer.Id
				));

				return ticket.Id;
			}

			return null;
		}

		public async Task<Unit> Handle(SetTicketTagsCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.FindAsync(request.TicketId);
			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			var brand = await _brandRepository.FindAsync(ticket.BrandId);
			if (brand == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket brand cannot be found."));
				return Unit.Value;
			}

			var brandTags = await _brandRepository.GetTags(ticket.BrandId, true, request.Level);
			var brandTagsIds = brandTags.Select(x => x.Id).ToHashSet();

			if (request.TagIds.Any(x => brandTagsIds.Contains(x) == false))
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"Cannot assign tags to ticket that are not associated with it's brand"));
				return Unit.Value;
			}

			if (brand.SubTagEnabled && !brand.MultiTagEnabled)
			{
				var mainTagIds = brandTags.Where(bt => !bt.ParentTagId.HasValue).Select(r => r.Id).ToHashSet();
				var subTagIds = brandTags.Where(bt => bt.ParentTagId.HasValue).Select(r => r.Id).ToHashSet();

				if (request.TagIds.Where(ids => mainTagIds.Contains(ids)).Count() > 1 ||
				    request.TagIds.Where(id => subTagIds.Contains(id)).Count() > 1)
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
						"Cannot assign multiple tags or subtags for this brand."));
					return Unit.Value;
				}

			}

			var ticketTagsActions = brandTags
				.Where(x => request.TagIds.Any(y => y == x.Id))
				.Where(x => x.Action != null)
				.Select(x => x.Action)
				.Distinct()
				.Count();

			if (ticketTagsActions > 1)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"Cannot assign tags to ticket that have more then 1 tag with action."));
				return Unit.Value;
			}

			var brandDisabledTags = await _ticketService.GetDisabledTagsOfTicketBrand(ticket);
			if (ticket.Status == TicketStatusEnum.Closed && ticket.ClosedBy == ClosedBy.EndChat &&
			    ticket.TagStatus == null
			    && request.TagIds.Any(x => brandDisabledTags.Contains(x)))
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"Cannot assign disabled tags to tickets that have been ended by customer."));
				return Unit.Value;
			}

			var previousTags = await _ticketTagRepository.GetAllAsync(x => x.TicketId == ticket.Id && x.Level == ticket.Level);
			var previousTagsIds = previousTags.Select(x => x.TagId).ToHashSet();
			var previousTagsNames = brandTags.Where(x => previousTags.Any(y => y.TagId == x.Id)).Select(x => x.Name).ToList();

			if (previousTagsIds.SetEquals(request.TagIds))
			{
				// nothing has changed - do not progress with the flow
				return Unit.Value;
			}

			await SetTicketTags(ticket.Id, request.TagIds, ticket.Level, ticket.AdvocateId ?? Guid.Empty, _timestampService.GetUtcTimestamp());
			
			var isSuccessful = await Commit();
			if (isSuccessful == false)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Cannot assign tags to ticket"));
				return Unit.Value;
			}
			
			var currentTagsNames = brandTags.Where(x => request.TagIds.Any(y => y == x.Id)).Select(x => x.Name).ToList();

			await _mediator.RaiseEvent(new TicketTagsChangedEvent(ticket.Id, ticket.BrandId, ticket.ReferenceId,
				(int)ticket.Level, previousTagsNames, currentTagsNames));

			//raise tag status change event.. enable/disable the mark as solved/ tagging complete button accordingly
			ticket = await _ticketRepository.GetTicketWithTaggingInfo( x=> x.Id == request.TicketId);
			await _mediator.RaiseEvent(new TicketTagsStatusChangedEvent(ticket.Id, ticket.BrandId, ticket.ReferenceId,
				ticket.ThreadId, ticket.AdvocateId, ticket.Advocate?.User?.FirstName,
				ticket.Advocate?.GetCsat(ticket.BrandId), ticket.IsTaggingComplete()));

			return Unit.Value;
		}

		public async Task<Guid?> Handle(ReserveEscalatedTicketCommand request, CancellationToken cancellationToken)
		{
			var advocateBrandIds = await _cachedBrandRepository.GetActiveBrandsIds(request.AdvocateId);
			Guid? ticketId;
			var reserved =
				await _ticketRepository.ReserveTicket(request.AdvocateId, advocateBrandIds, TicketLevel.SuperSolver);

			if (reserved)
			{
				ticketId = await _ticketRepository.GetReservedTicketId(request.AdvocateId);

				if (!ticketId.HasValue)
				{
					return null;
				}
			}
			else
			{
				await _mediator.RaiseEvent(new TicketReservationFailedEvent(request.AdvocateId,
					advocateBrandIds.ToArray(), TicketLevel.SuperSolver));
				return null;
			}

			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == ticketId);
			var previousAdvocateId = ticket.AdvocateId;
			var timestamp = _timestampService.GetUtcTimestamp();

			// Super solvers get assigned a ticket automatically.
			ticket.Accept(timestamp);

			if (await Commit())
			{
				// Note - TicketAdvocateChangedEvent should be the first one to be called as it clears cache for the respective ticket.
				await _mediator.RaiseEvent(new TicketAdvocateChangedEvent(ticket.Id, ticket.Status, previousAdvocateId,
					ticket.AdvocateId, ticket.Advocate?.User?.FirstName, ticket.Advocate?.GetCsat(ticket.BrandId)));

				// Raise integration event
				// Raise integration event
				await _bus.Publish<ITicketAdvocateChangedEvent>(
					new
					{
						TicketId = ticket.Id,
						TicketStatus = (int)ticket.Status,
						OldAdvocateId = previousAdvocateId,
						NewAdvocateId = ticket.AdvocateId,
						NewAdvocateFirstName = ticket.Advocate?.User?.FirstName,
						NewAdvocateCsat = ticket.Advocate?.GetCsat(ticket.BrandId)
					}, cancellationToken);

				await _mediator.RaiseEvent(new TicketAcceptedEvent(ticket.Id, ticket.BrandId, ticket.ReferenceId,
					ticket.IsPractice, ticket.AdvocateId.Value,
					ticket.Advocate.User.FirstName, ticket.Advocate.GetCsat(ticket.BrandId), ticket.Advocate.Super,
					ticket.Source?.Name, ticket.Level, ticket.EscalationReason, ticket.TransportType,
					ticket.FirstAssignedDate.Value, ticket.AssignedDate.Value, ticket.Customer.Id
				));

				return ticket.Id;
			}

			return null;
		}

		public async Task<Unit> Handle(AcceptTicketCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);
			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));

				return Unit.Value;
			}

			var advocate = await _advocateRepository.GetFullAdvocateAsync(a => a.Id == ticket.AdvocateId);
			var oldStatus = ticket.Status;

			ticket.Accept(_timestampService.GetUtcTimestamp());

			// Track ticket
			ticket.Track(ticket.Advocate.User, _ipAddress, _userAgent, request.MessageType,
				_timestampService.GetUtcTimestamp());

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new TicketAcceptedEvent(ticket.Id, ticket.BrandId, ticket.ReferenceId,
					ticket.IsPractice,
					ticket.AdvocateId.Value, ticket.Advocate.User.FirstName,
					ticket.Advocate.GetCsat(ticket.BrandId),
					ticket.Advocate.Super,
					ticket.Source?.Name,
					ticket.Level,
					ticket.EscalationReason,
					ticket.TransportType,
					ticket.FirstAssignedDate.Value, ticket.AssignedDate.Value, ticket.Customer.Id
				));

				// No need to publish TicketAdvocateChanged because it is the same one who reserved it.
				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be accepted"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(RejectTicketCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);
			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			var exists = await _ticketRepository.RejectReasonsExist(request.RejectReasonIds);
			if (!exists)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"Incorrect rejection reasons ids passed"));
				return Unit.Value;
			}

			var previousAdvocateId = ticket.AdvocateId;
			ticket.Reject(request.RejectReasonIds, _timestampService.GetUtcTimestamp());

			// Track ticket
			ticket.Track(ticket.Advocate.User, _ipAddress, _userAgent, request.MessageType,
				_timestampService.GetUtcTimestamp());

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				// Note - TicketAdvocateChangedEvent should be the first one to be called as it clears cache for the respective ticket.
				// If the ticket is practice, we notify the old solver only (the new solver is gonna be the same anyway)
				// so the FE can simulates as if the ticket was removed to be picked up by someone else)
				await _mediator.RaiseEvent(new TicketAdvocateChangedEvent(ticket.Id, ticket.Status, previousAdvocateId,
					ticket.IsPractice ? null : ticket.AdvocateId,
					ticket.IsPractice ? null : ticket.Advocate?.User?.FirstName,
					ticket.IsPractice ? null : ticket.Advocate?.GetCsat(ticket.BrandId)));

				// Raise integration event
				// Raise integration event
				await _bus.Publish<ITicketAdvocateChangedEvent>(
					new
					{
						TicketId = ticket.Id,
						TicketStatus = (int)ticket.Status,
						OldAdvocateId = previousAdvocateId,
						NewAdvocateId = ticket.AdvocateId,
						NewAdvocateFirstName = ticket.Advocate?.User?.FirstName,
						NewAdvocateCsat = ticket.Advocate?.GetCsat(ticket.BrandId)
					}, cancellationToken);

				await _mediator.RaiseEvent(new TicketRejectedEvent(ticket.Id, ticket.BrandId, ticket.Level,
					previousAdvocateId.Value,
					ticket.Source?.Id, ticket.CreatedDate, ticket.RejectionCount, ticket.Customer.Id));

				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be rejected"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetReturningCustomerStateCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			ticket.SetReturningCustomerState(request.State);
			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				if (ticket.ReturningCustomerState == ReturningCustomerState.CustomerReturned)
				{
					await _mediator.RaiseEvent(new TicketMarkedAsReturningCustomerEvent(ticket.Id, ticket.BrandId,
						ticket.Status, ticket.Advocate?.User?.FirstName, ticket.Customer.FirstName));
				}

				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
				"Ticket cannot be marked as returning customer"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetCustomerRepeatedCountCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			ticket.SetRepeatedCustomerCount();
			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
				$"Repeated Count cannot be updated for ticket {ticket.Id}"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetNotificationResumptionStateCommand request,
			CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			ticket.SetNotificationResumptionState(request.State);
			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				if (ticket.NotificationResumptionState == NotificationResumptionState.CustomerResumed)
				{
					await _mediator.RaiseEvent(new TicketChatResumedEvent(ticket.Id, ticket.BrandId, ticket.Status,
						ticket.Advocate?.User?.FirstName, ticket.Customer.FirstName));
				}

				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
				"Ticket cannot be marked as returning customer"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetTicketSposCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			var previousStatus = ticket.SposLead;

			ticket.SetSpos(request.SposLead, request.SposDetails);

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				//raise tag status change event.. enable the mark as solved/ tagging complete button accordingly
				await _mediator.RaiseEvent(new TicketTagsStatusChangedEvent(ticket.Id, ticket.BrandId,
					ticket.ReferenceId,
					ticket.ThreadId, ticket.AdvocateId, ticket.Advocate?.User?.FirstName,
					ticket.Advocate?.GetCsat(ticket.BrandId), ticket.IsTaggingComplete()));
				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
				"Ticket SPOS details cannot be saved"));
			return Unit.Value;
		}

		public async Task<Unit> Handle(SolveTicketCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			var oldStatus = ticket.Status;
			ticket.Solve(_timestampService.GetUtcTimestamp());

			// Track ticket
			ticket.Track(ticket.Advocate.User, _ipAddress, _userAgent, request.MessageType,
				_timestampService.GetUtcTimestamp());

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new TicketSolvedEvent(ticket.Id, ticket.BrandId, ticket.ReferenceId,
					ticket.ThreadId, ticket.IsPractice, ticket.Source?.Name,
					ticket.AdvocateId.Value, ticket.Advocate.User.FirstName, ticket.Advocate.GetCsat(ticket.BrandId),
					ticket.TransportType, ticket.Brand.WaitMinutesToClose, ticket.Customer.Id));

				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be solved"));

			return Unit.Value;
		}

		public async Task<Ticket> Handle(ExpireTicketReservationCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return null;
			}

			var previousAdvocateId = ticket.AdvocateId;
			ticket.CancelReservation();
			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				// TOOD: Same as Reject. If the ticket is for practice, the advocate id won't change.
				// Note - TicketAdvocateChangedEvent should be the first one to be called as it clears cache for the respective ticket.
				await _mediator.RaiseEvent(new TicketAdvocateChangedEvent(ticket.Id, ticket.Status, previousAdvocateId,
					ticket.IsPractice ? null : ticket.AdvocateId,
					ticket.IsPractice ? null : ticket.Advocate?.User?.FirstName,
					ticket.IsPractice ? null : ticket.Advocate?.GetCsat(ticket.BrandId)));

				// Raise integration event
				// Raise integration event
				await _bus.Publish<ITicketAdvocateChangedEvent>(
					new
					{
						TicketId = ticket.Id,
						TicketStatus = (int)ticket.Status,
						OldAdvocateId = previousAdvocateId,
						NewAdvocateId = ticket.AdvocateId,
						NewAdvocateFirstName = ticket.Advocate?.User?.FirstName,
						NewAdvocateCsat = ticket.Advocate?.GetCsat(ticket.BrandId)
					}, cancellationToken);

				return ticket;
			}

			return null;
		}

		public async Task<Unit> Handle(AbandonTicketCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			var brand = await _brandRepository.GetFirstOrDefaultAsync(
				predicate: x => x.Id == ticket.BrandId,
				include: x => x.Include(i => i.AbandonReasons)
			);

			var abandonReasonIds = request.AbandonReasonIds;
			if (request.AutoAbandoned)
			{
				abandonReasonIds = brand.AbandonReasons
					.Where(x => x.IsAutoAbandoned)
					.Select(x => x.Id)
					.ToArray();
			}

			var exists = abandonReasonIds.All(x => brand.AbandonReasons.Select(y => y.Id).Contains(x));
			if (exists == false)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"Incorrect abandon reasons ids passed"));
				return Unit.Value;
			}

			var action = brand.AbandonReasons
				.Where(x => abandonReasonIds.Contains(x.Id))
				.Where(x => x.Action != null)
				.Select(x => x.Action)
				.FirstOrDefault();

			var previousAdvocateId = ticket.AdvocateId;
			var abandonedBy = ticket.Advocate?.User?.FirstName;

			// TODO: Pass the latest price when implementing the ticket price Abandon(latestPrice.Price).
			ticket.Abandon(abandonReasonIds, brand.TicketPrice,
				_brandService.CalculateTicketFee(brand.TicketPrice, brand.FeePercentage),
				_timestampService.GetUtcTimestamp());

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				//Same as in Reject. If the ticket is for practice, the advocate id won't change.
				// Note - TicketAdvocateChangedEvent should be the first one to be called as it clears cache for the respective ticket.
				await _mediator.RaiseEvent(new TicketAdvocateChangedEvent(ticket.Id, ticket.Status, previousAdvocateId,
					ticket.IsPractice ? null : ticket.AdvocateId,
					ticket.IsPractice ? null : ticket.Advocate?.User?.FirstName,
					ticket.IsPractice ? null : ticket.Advocate?.GetCsat(ticket.BrandId)));

				// Raise integration event
				// Raise integration event
				await _bus.Publish<ITicketAdvocateChangedEvent>(
					new
					{
						TicketId = ticket.Id,
						TicketStatus = (int)ticket.Status,
						OldAdvocateId = previousAdvocateId,
						NewAdvocateId = ticket.AdvocateId,
						NewAdvocateFirstName = ticket.Advocate?.User?.FirstName,
						NewAdvocateCsat = ticket.Advocate?.GetCsat(ticket.BrandId)
					}, cancellationToken);

				await _mediator.RaiseEvent(new TicketAbandonedEvent(ticket.Id, ticket.BrandId, ticket.Level,
					ticket.ReferenceId,
					ticket.IsPractice,
					previousAdvocateId.Value, ticket.Source?.Id, ticket.Source?.Name, ticket.CreatedDate,
					ticket.AbandonedCount,
					abandonedBy, request.AutoAbandoned, action,
					ticket.GetStatusDuration(TicketStatusEnum.New, _timestampService), ticket.Customer.Id)
				);

				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be abandoned"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(ReopenTicketCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);
			var advocate = await _advocateRepository.GetFirstOrDefaultAsync(s => s, a => a.Id == ticket.AdvocateId,
				include: src => src.Include(i => i.User).Include(i => i.Brands));
			var prevAdvocateName = advocate?.User?.FirstName;

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));

				return Unit.Value;
			}

			ticket.Reopen(_timestampService.GetUtcTimestamp());

			_ticketRepository.Update(ticket);

			var advocateBlocked = advocate?.Status == AdvocateStatus.Blocked ||
			                      advocate?.Brands.Count(x => x.BrandId == ticket.BrandId && x.Blocked == false) == 0;

			if (await Commit())
			{
				await _mediator.RaiseEvent(new TicketReopenedEvent(ticket.Id, ticket.BrandId,
					ticket.AdvocateId, ticket.Advocate.User.FirstName, ticket.Advocate.GetCsat(ticket.BrandId),
					ticket.Customer.FirstName,
					ticket.ReferenceId,
					ticket.IsPractice, ticket.Source?.Name,
					ticket.Source?.Id, ticket.CreatedDate,
					ticket.ReturningCustomerState, ticket.NotificationResumptionState,
					ticket.TransportType, advocateBlocked, ticket.Customer.Id));

				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be reopened"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(CloseTicketCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);
			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			if (ticket.Status == TicketStatusEnum.Closed)
			{
				_logger.LogWarning("Ticket {0} has already been closed and cannot be closed by {1}", request.TicketId,
					request.ClosedBy);
				return Unit.Value;
			}

			if (ticket.Level == TicketLevel.PushedBack)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					$"Ticket {TicketLevel.PushedBack} cannot be closed."));
				return Unit.Value;
			}

			if (request.ClosedBy == ClosedBy.EndChat && !ticket.Brand.EndChatEnabled)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					$"{request.ClosedBy} feature is not enabled for brand {ticket.Brand.Id}."));
				return Unit.Value;
			}

			var previousAdvocateId = ticket.AdvocateId;
			ticket.Close(_timestampService.GetUtcTimestamp(), request.ClosedBy);

			var disabledTags = new List<Guid>();

			// Track ticket
			if (request.ClosedBy == Domain.Enums.ClosedBy.Customer)
			{
				ticket.Track(ticket.Advocate.User, _ipAddress, _userAgent, request.MessageType,
					_timestampService.GetUtcTimestamp());
			}
			// workflow for end chat ticket
			else if (request.ClosedBy == ClosedBy.EndChat && ticket.AreTagsAvailable())
			{
				ticket.TagStatus = null;
				disabledTags = await _ticketService.GetDisabledTagsOfTicketBrand(ticket);
				var currentlySelectedDisabledTags = ticket.Tags.Where(x => disabledTags.Contains(x.TagId)).ToList();

				currentlySelectedDisabledTags.ForEach(tag =>
				{
					ticket.Tags.Remove(tag);
				});

				if (currentlySelectedDisabledTags.Any())
				{
					//raise tag status change event.. enable the mark as solved/ tagging complete button accordingly
					await _mediator.RaiseEvent(new TicketTagsStatusChangedEvent(ticket.Id, ticket.BrandId,
						ticket.ReferenceId,
						ticket.ThreadId, ticket.AdvocateId, ticket.Advocate?.User?.FirstName,
						ticket.Advocate?.GetCsat(ticket.BrandId), ticket.IsTaggingComplete()));
				}
			}

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new TicketClosedEvent()
				{
					TicketId = ticket.Id,
					BrandId = ticket.BrandId,
					ReferenceId = ticket.ReferenceId,
					ThreadId = ticket.ThreadId,
					IsPractice = ticket.IsPractice,
					SourceName = ticket.Source?.Name,
					CustomerId = ticket.Customer.Id,
					AdvocateId = ticket.AdvocateId,
					AdvocateFirstName = ticket.Advocate?.User?.FirstName,
					AdvocateCsat = ticket.Advocate?.GetCsat(ticket.BrandId),
					ClosedBy = request.ClosedBy,
					TransportType = ticket.TransportType,
					TagStatus = ticket.TagStatus
				});

				if (previousAdvocateId != ticket.AdvocateId)
				{
					// notify the concerned part if the advocate has been changed,
					// because normal ticket (not escalated) won't make the advocate changes.
					// Note - TicketAdvocateChangedEvent should be the first one to be called as it clears cache for the respective ticket.
					await _mediator.RaiseEvent(new TicketAdvocateChangedEvent(ticket.Id, ticket.Status,
						previousAdvocateId,
						ticket.AdvocateId, ticket.Advocate?.User?.FirstName, ticket.Advocate?.GetCsat(ticket.BrandId)));

					// Raise integration event
					// Raise integration event
					await _bus.Publish<ITicketAdvocateChangedEvent>(
						new
						{
							TicketId = ticket.Id,
							TicketStatus = (int)ticket.Status,
							OldAdvocateId = previousAdvocateId,
							NewAdvocateId = ticket.AdvocateId,
							NewAdvocateFirstName = ticket.Advocate?.User?.FirstName,
							NewAdvocateCsat = ticket.Advocate?.GetCsat(ticket.BrandId)
						}, cancellationToken);

					if (ticket.IsPendingStatusNotificationRequired)
					{
						await _mediator.RaiseEvent(new TicketPendingStatusUpdatedEvent(ticket.Id, ticket.AdvocateId,
							ticket.BrandId, ticket.Status));
					}
				}
				else if (ticket.TagStatus != null && ticket.IsPendingStatusNotificationRequired)
				{
					await _mediator.RaiseEvent(new TicketPendingStatusUpdatedEvent(ticket.Id, ticket.EscalatedById,
						ticket.BrandId, ticket.Status));
				}

				/// To disable tags when customer ends chat
				if (disabledTags.Any())
				{
					await _mediator.RaiseEvent(new TicketTagsDisabledEvent(ticket.Id, ticket.BrandId,
						ticket.AdvocateId.Value, disabledTags));
				}

				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be closed"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(CompleteTicketCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			if (!(ticket.Status == TicketStatusEnum.Closed && ticket.TagStatus == null &&
			      ticket.ClosedBy == ClosedBy.EndChat))
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"Cannot set completion for a ticket that is not ended by customer."));
				return Unit.Value;
			}

			var previousAdvocateId = ticket.AdvocateId;
			ticket.TagStatus = request.TagStatus;

			if (ticket.Level == TicketLevel.SuperSolver && ticket.EscalatedById != null &&
			    ((ticket.CorrectlyDiagnosed.HasValue && ticket.CorrectlyDiagnosed.Value) ||
			     ticket.TagStatus == TicketTagStatus.Unknown))
			{
				ticket.AdvocateId = ticket.EscalatedById;
				ticket.Level = TicketLevel.Regular;
			}

			if (ticket.TagStatus == TicketTagStatus.Unknown)
			{
				ticket.Tags = null;
				ticket.TicketCategory = null;
				ticket.SposDetails = string.Empty;
				ticket.SposEmailSent = null;
				ticket.SposLead = null;
				ticket.ValidTransfer = null;
				ticket.CorrectlyDiagnosed = null;
			}

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				await _workflowService.StartWorkflow(WorkflowKeys.CompleteTicketKey, WorkflowKeys.CompleteTicketVersion
						, new CompleteTicketWorkflowModel()
						{
							TicketId = ticket.Id,
							AdvocateId = ticket.AdvocateId,
							AdvocateFirstName = ticket.Advocate?.User?.FirstName,
							AdvocateCsat = ticket.Advocate?.GetCsat(ticket.BrandId),
							ClosedBy = ticket.ClosedBy.Value,
							TransportType = ticket.TransportType,
							CustomerId = ticket.Customer.Id,
							TagStatus = request.TagStatus
						});

				if (ticket.SposLead == true && ticket.SposEmailSent != true)
				{
					var sposNotification = ticket.Tags.All(x =>
						x.Tag.SposNotificationEnabled == null || x.Tag.SposNotificationEnabled == true);
					await _mediator.RaiseEvent(new TicketSposLeadSetEvent(ticket.Id, ticket.Customer.Id, ticket.BrandId,
						sposNotification));
				}

				if (previousAdvocateId != ticket.AdvocateId)
				{
					// notify the concerned part if the advocate has been changed,
					// because normal ticket (not escalated) won't make the advocate changes.
					// Note - TicketAdvocateChangedEvent should be the first one to be called as it clears cache for the respective ticket.
					await _mediator.RaiseEvent(new TicketAdvocateChangedEvent(ticket.Id, ticket.Status,
						previousAdvocateId,
						ticket.AdvocateId, ticket.Advocate?.User?.FirstName, ticket.Advocate?.GetCsat(ticket.BrandId)));

					if (ticket.IsPendingStatusNotificationRequired || ticket.TagStatus == TicketTagStatus.Unknown)
					{
						await _mediator.RaiseEvent(new TicketPendingStatusUpdatedEvent(ticket.Id, ticket.AdvocateId,
							ticket.BrandId, ticket.Status));
					}
				}
				else
				{
					if (ticket.IsPendingStatusNotificationRequired)
					{
						await _mediator.RaiseEvent(new TicketPendingStatusUpdatedEvent(ticket.Id, ticket.EscalatedById,
							ticket.BrandId, ticket.Status));
					}
				}

				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
				"Ticket tagging could not be completed"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(EscalateTicketCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);
			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			var originalStatus = ticket.Status;
			var previousAdvocateId = ticket.AdvocateId;

			ticket.Escalate(request.Reason, request.Level, _timestampService.GetUtcTimestamp());

			// track ticket
			if (ticket.AdvocateId.HasValue)
			{
				ticket.Track(ticket.Advocate.User, _ipAddress, _userAgent, request.MessageType,
					_timestampService.GetUtcTimestamp());
			}

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				// Note - TicketAdvocateChangedEvent should be the first one to be called as it clears cache for the respective ticket.
				if (previousAdvocateId != ticket.AdvocateId)
				{
					await _mediator.RaiseEvent(new TicketAdvocateChangedEvent(ticket.Id, ticket.Status,
						previousAdvocateId,
						ticket.AdvocateId, ticket.Advocate?.User?.FirstName, ticket.Advocate?.GetCsat(ticket.BrandId)));

					// Raise integration event
					await _bus.Publish<ITicketAdvocateChangedEvent>(
						new
						{
							TicketId = ticket.Id,
							TicketStatus = (int)ticket.Status,
							OldAdvocateId = previousAdvocateId,
							NewAdvocateId = ticket.AdvocateId,
							NewAdvocateFirstName = ticket.Advocate?.User?.FirstName,
							NewAdvocateCsat = ticket.Advocate?.GetCsat(ticket.BrandId)
						}, cancellationToken);
				}

				await _mediator.RaiseEvent(new TicketEscalatedEvent(ticket.Id, ticket.AdvocateId, ticket.Level,
					ticket.Advocate?.User?.FirstName,
					ticket.Advocate?.GetCsat(ticket.BrandId), ticket.BrandId,
					ticket.ReferenceId, ticket.ThreadId, ticket.IsPractice, ticket.Source?.Name, originalStatus,
					request.Reason, ticket.Customer.Id
				));

				if (ticket.IsPendingStatusNotificationRequired)
				{
					await _mediator.RaiseEvent(new TicketPendingStatusUpdatedEvent(ticket.Id, ticket.EscalatedById,
						ticket.BrandId, ticket.Status));
				}

				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be escalated"));
				return Unit.Value;
		}

		public async Task<Unit> Handle(SetTicketComplexityCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));

				return Unit.Value;
			}

			if (ticket.Complexity != null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"Ticket complexity already updated."));
				return Unit.Value;
			}

			ticket.SetComplexity(request.Complexity);

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new TicketComplexitySetEvent(ticket.Id, ticket.AdvocateId, ticket.IsPractice));
				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be closed"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(IncreaseTicketChaserEmailsCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFirstOrDefaultAsync(
								predicate: i => i.Id == request.TicketId);

			ticket.ChaserEmails = ticket.ChaserEmails.HasValue ? ticket.ChaserEmails + 1 : 1;
			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				return Unit.Value;
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetTicketCsatCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));

				return Unit.Value;
			}

			if (ticket.Csat != null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket CSAT already updated."));
				return Unit.Value;
			}

			ticket.SetCSAT(request.Csat, _timestampService.GetUtcTimestamp());

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new TicketCsatSetEvent(ticket.Id, ticket.AdvocateId.Value, ticket.BrandId, ticket.Brand.Name, ticket.IsPractice, request.Csat, ticket.TransportType));
				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be closed"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.FindAsync(request.TicketId);

			if (ticket == null || !ticket.IsPractice)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"Ticket cannot be found or is not in practice"));

				return Unit.Value;
			}

			_ticketRepository.Delete(ticket);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new TicketDeletedEvent(ticket.Id, ticket.IsPractice));

				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
				"Ticket cannot be deleted"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(UpdateTicketMessageStatisticsCommand request, CancellationToken cancellationToken)
		{
			using (var t = await BeginTransaction()) // do changes in a transaction as we read & update read data here
			{
				var ticket = await _ticketRepository.FindAsync(request.TicketId);
				ticket.SetSolverResponseTime(request.CreatedDate, request.SenderType);
				ticket.SetMessageDatesDetails(request.CreatedDate, request.SenderType);
				_ticketRepository.Update(ticket);
				await Commit();
				await t.CommitAsync();
				return Unit.Value;
			}
		}
		public async Task<Unit> Handle(SendTicketCreatedEmail request, CancellationToken cancellationToken)
		{
			var localizationProvider = _localizationProviderFactory.GetLocalizationProvider(request.Culture);
			var brand = await _brandRepository.FindAsync(request.BrandId);
			var email = await _userRepository.GetFirstOrDefaultAsync(u => u.Email, u => u.Id == request.CustomerId);
			var templateModel = new
			{
				BrandName = brand.Name,
				Question = request.Question.Truncate(30, true)
			};
			var emailSubject = _templateService.Render(localizationProvider.Resources.Emails.TicketCreated.Subject, templateModel);
			var sender = _templateService.Render(localizationProvider.Resources.Emails.Sender, templateModel);
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

			await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
			{
				Culture = request.Culture,
				MailTo = email,
				Subject = emailSubject,
				Template = EmailTemplates.TicketCreated.ToString(),
				Model = new Dictionary<string, object>
				{
					{"Question", request.Question.Truncate(75)},
					{"BrandName", brand.Name},
					{"BrandLogoUrl", brand.Logo}
				},
				SenderName = sender
			}, cancellationToken);

			return Unit.Value;
		}

		public async Task<bool> Handle(SendFirstAdvocateResponseInChatEmailCommand request,
			CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.FindAsync(request.BrandId);
			var advocateFirstName =
				await _userRepository.GetSingleOrDefaultAsync(selector: b => b.FirstName,
					b => b.Id == request.AdvocateId);
			var emailSubject =
				$"{brand.Name} Solver has replied to your question: {request.Question.Truncate(30, true)}";
			var chatUrl = await _ticketUrlService.GetChatUrl(request.TicketId, true);
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

			await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
			{
				MailTo = request.Email,
				Subject = emailSubject,
				Template = EmailTemplates.FirstAdvocateResponseInChat.ToString(),
				Model = new Dictionary<string, object>
				{
					{"Question", request.Question.Truncate(75)},
					{"chatUrl", chatUrl},
					{"BrandName", brand.Name},
					{"BrandLogoUrl", brand.Logo},
					{"advocateFirstName", advocateFirstName},
					{"CustomerFirstName", request.CustomerFirstName}
				},
				SenderName = $"{brand.Name} Support"
			}, cancellationToken);

			return true;
		}

		public async Task<bool> Handle(SendTicketClosedEmailCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.FindAsync(request.BrandId);
			var advocateFirstName =
				await _userRepository.GetSingleOrDefaultAsync(selector: b => b.FirstName,
					b => b.Id == request.AdvocateId);
			var truncatedQuestion = request.Question.Truncate(30, true);
			var emailSubject = $"Question closed: {truncatedQuestion}";
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

			await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
			{
				MailTo = request.Email,
				Subject = emailSubject,
				Template = EmailTemplates.TicketClosed_Chat.ToString(),
				Model = new Dictionary<string, object>
				{
					{"Question", request.Question},
					{"CreatedDate", request.CreatedDate.ToString("MMM dd, hh:mm")},
					{"BrandName", brand.Name},
					{"BrandLogoUrl", brand.Logo},
					{"advocateFirstName", advocateFirstName},
				},
				SenderName = $"{brand.Name} Support",
				Attachment = new EmailAttachment
				{
					ContentType = MediaTypeNames.Text.Plain,
					Filename = $"ticket-transcript-{request.TicketId}.txt",
					Content = request.Conversation,
				}
			}, cancellationToken);

			return true;
		}

		public async Task<Unit> Handle(ScheduleChatReminderCommand request, CancellationToken cancellationToken)
		{
			var (advocateFirstName, brandLogo, brandName, customerId, customerEmail, question) =
				await _ticketRepository.GetFirstOrDefaultAsync(
					selector: x => Tuple.Create(
						x.Advocate.User.FirstName,
						x.Brand.Logo,
						x.Brand.Name,
						x.Customer.Id,
						x.Customer.Email,
						x.Question
					).ToValueTuple(),
					predicate: x => x.Id == request.TicketId);

			var emailSubject = $"{brandName} Solver has replied to your question: {question.Truncate(30, true)}";
			var chatUrl = await _ticketUrlService.GetChatUrl(request.TicketId, true);
			var cmd = new SendChatReminderCommand
			{
				TicketId = request.TicketId,
				Subject = emailSubject,
				BrandLogoUrl = brandLogo,
				BrandName = brandName,
				Question = question.Truncate(75),
				AdvocateFirstName = advocateFirstName,
				ChatUrl = chatUrl,
				CustomerEmail = customerEmail,
				EmailLogoLocation = _emailTemplateOptions.EmailLogoLocation
			};

			var reminderTimestamp = _timestampService.GetUtcTimestamp()
				.AddSeconds(_emailTemplateOptions.ChatReminderDelaySeconds);
			await _schedulerService.CancelScheduledJob(cmd); // cancel any previous reminders for this ticket
			await _schedulerService.ScheduleJob(cmd, reminderTimestamp);
			return Unit.Value;
		}

		private async Task<Ticket> PrepareTicket(CreateTicketCommand request, DateTime utcTimeStamp, decimal? overridenPrice = null)
		{
			// If the customer has already created a ticket before, we use his user again.
			var customer = await _userRepository.GetFirstOrDefaultAsync(user => user, p => p.Email == request.Email.ToLower(),
				disableTracking: false);

			// if we couldn't find a customer with the same email, we create one.
			if (customer == null)
			{
				customer = new User(Guid.NewGuid(), request.FirstName, request.LastName, request.Email, null);
			}

			// fetch ticket source
			TicketSource source = null;
			if (request.Source != null)
			{
				source = await _ticketSourceRepository.GetSingleOrDefaultAsync(predicate: x => x.Name == request.Source);
				if (source == null)
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Unknown ticket source"));
					return null;
				}
			}

			var brand = await _brandRepository.FindAsync(request.BrandId);
			var ticketPrice = overridenPrice ?? brand.TicketPrice;
			var fee = _brandService.CalculateTicketFee(ticketPrice, brand.FeePercentage);

			var brandDetails = await _cachedBrandRepository.GetAsync(request.BrandId);
			var metadata = PrepareMetadata(brandDetails.FormFields, request.Metadata);

			var ticket = new Ticket(customer, request.Question, brand, ticketPrice, fee,
				request.ReferenceId, request.ThreadId, source, brand.DefaultCulture, request.TransportType, TicketLevel.Regular,
				request.PracticingAdvocateId, metadata, request.ProbingAnswers,
				utcTimeStamp);

			await _brandMetadataService.RouteToLevel(ticket, metadata);

			return ticket;
		}

		private IEnumerable<TicketMetadataItem> PrepareMetadata(IEnumerable<Cached.BrandFormField> brandFormFields, IReadOnlyDictionary<string, string> metadata)
		{
			return from md in metadata ?? new Dictionary<string, string>()
				   join ff in brandFormFields on md.Key equals ff.Name into det
				   from bff in det.DefaultIfEmpty()
				   select new TicketMetadataItem(md.Key, md.Value, bff?.Id, bff?.Order);
		}

		public async Task<Unit> Handle(SetTicketNpsCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.FindAsync(request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));

				return Unit.Value;
			}

			var brand = await _brandRepository.FindAsync(ticket.BrandId);

			if (ticket.Nps != null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Ticket {ticket.Id} NPS already updated."));
				return Unit.Value;
			}

			if (!brand.NpsEnabled)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Brand NPS is not enabled for ticket {ticket.Id}"));
				return Unit.Value;
			}

			ticket.SetNps(request.Nps, _timestampService.GetUtcTimestamp());

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new TicketNpsSetEvent(ticket.Id, ticket.AdvocateId.Value, ticket.BrandId,
					ticket.IsPractice, request.Nps, ticket.TransportType));
				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket NPS cannot be set"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetFraudStatusCommand request, CancellationToken cancellationToken)
		{
			var tickets = await _ticketRepository.GetAllAsync(x => request.TicketIds.Contains(x.Id));

			if (tickets.Count == 0)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Tickets cannot be found."));
				return Unit.Value;
			}

			var tasks = new List<Task>();
			tasks.AddRange(tickets.Select(t => BulkFraudStatusUpdate(t, request.FraudStatus)));
			await Task.WhenAll(tasks);

			_ticketRepository.Update(tickets);

			if (await Commit())
			{
				// TODO if necessary in future, raise event
				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Cannot update fraud status for the selected tickets."));
			return Unit.Value;
		}

		public async Task<Unit> Handle(UpdateTicketFraudCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.FindAsync(request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Ticket {request.TicketId} cannot be found."));
				return Unit.Value;
			}

			ticket.SetFraudDetection(request.RiskLevel, request.Risks);

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Ticket {request.TicketId} updated."));
				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Ticket {request.TicketId} fraud status cannot be set"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(SendTicketSposEmail request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);

			ticket.SposEmailSent = true;
			_ticketRepository.Update(ticket);

			if (await Commit() && request.SendSposMail)
			{
				var brand = await _brandRepository.GetFirstOrDefaultAsync(
						predicate: x => x.Id == request.BrandId,
						include: x => x.Include(i => i.FormFields)
					);

				var emailSubject = $"{ticket.Customer.FullName} sales leads from {brand.Name} Crowd (Solv) support";

				var metaData = string.Empty;
				foreach (var key in brand.FormFields)
				{
					metaData = metaData + key.Title + " : " + ticket.Metadata?.Where(x => x.Key == key.Name).Select(x => x.Value).FirstOrDefault() + "<br />";
				}

				var transcript = await _chatService.GetTranscript(request.TicketId);

				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));
				await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
				{
					MailTo = brand.SposEmail,
					Subject = emailSubject,
					Template = EmailTemplates.TicketSposEmail.ToString(),
					Model = new Dictionary<string, object>
						{
							{"SposDetails", ticket.SposDetails},
							{"CustomerFirstName", ticket.Customer.FirstName},
							{"CustomerName", ticket.Customer.FullName},
							{"CustomerEmail", ticket.Customer.Email},
							{"TicketMetaData", metaData},
							{"SolvAgentId", ticket.Advocate.User.Email},
							{"SolvTicketId", request.TicketId},
						},
					SenderName = $"{brand.Name} Support",
					Attachment = new EmailAttachment
					{
						ContentType = MediaTypeNames.Text.Plain,
						Filename = $"ticket-transcript-{request.TicketId}.txt",
						Content = transcript
					}
				}, cancellationToken);
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetTicketCategoryCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);
			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			if (!ticket.Brand.CategoryEnabled)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					$"Category selection not applicable for brand {ticket.BrandId}"));
				return Unit.Value;
			}

			var brandCategories = await _brandRepository.GetCategories(ticket.BrandId, true);
			var brandCategoryIds = brandCategories.Select(x => x.Id).ToHashSet();

			if (!brandCategoryIds.Where(bc => bc == request.CategoryId).Any())
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"Cannot assign category to ticket that is not associated with it's brand"));
				return Unit.Value;
			}

			if (ticket.TicketCategory?.CategoryId == request.CategoryId && ticket.AdvocateId == request.AdvocateId)
			{
				// nothing has changed - do not progress with the flow
				return Unit.Value;
			}

			ticket.TicketCategory = new TicketCategory(request.TicketId, request.CategoryId, request.AdvocateId);

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				if (ticket.IsTaggingComplete())
				{
					//raise tag status change event.. enable the mark as solved/ tagging complete button accordingly
					await _mediator.RaiseEvent(new TicketTagsStatusChangedEvent(ticket.Id, ticket.BrandId, ticket.ReferenceId,
								ticket.ThreadId, ticket.AdvocateId, ticket.Advocate?.User?.FirstName,
								ticket.Advocate?.GetCsat(ticket.BrandId), true));
				}
				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
				$"Cannot assign category - {request.CategoryId} to ticket - {request.TicketId}"));
			return Unit.Value;
		}

		public async Task<bool> Handle(SetTicketDiagnosisCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return false;
			}

			if (ticket.EscalatedById == null && request.CorrectlyDiagnosed != null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Ticket {request.TicketId} was not escalated by L1 solver, so assigning diagnosis result is not valid operation."));
				return false;
			}

			ticket.CorrectlyDiagnosed = request.CorrectlyDiagnosed;
			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				if (ticket.IsTaggingComplete())
				{
					//raise tag status change event.. enable the mark as solved/ tagging complete button accordingly
					await _mediator.RaiseEvent(new TicketTagsStatusChangedEvent(ticket.Id, ticket.BrandId, ticket.ReferenceId,
								ticket.ThreadId, ticket.AdvocateId, ticket.Advocate?.User?.FirstName,
								ticket.Advocate?.GetCsat(ticket.BrandId), true));
				}
				return true;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket diagnosis cannot be saved"));
			return false;
		}

		public async Task<bool> Handle(SetTicketValidTransferCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetFullTicket(p => p.Id == request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return false;
			}

			if (ticket.Brand.ValidTransferEnabled)
			{
				if (ticket.Level != TicketLevel.SuperSolver)
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Ticket {request.TicketId} was not escalated, so assigning valid transfer result is not valid operation."));
					return false;
				}

				ticket.ValidTransfer = request.IsValidTransfer;
				_ticketRepository.Update(ticket);

				if (await Commit())
				{
					if (ticket.IsTaggingComplete())
					{
						//raise tag status change event.. enable the mark as solved/ tagging complete button accordingly
						await _mediator.RaiseEvent(new TicketTagsStatusChangedEvent(ticket.Id, ticket.BrandId, ticket.ReferenceId,
									ticket.ThreadId, ticket.AdvocateId, ticket.Advocate?.User?.FirstName,
									ticket.Advocate?.GetCsat(ticket.BrandId), true));
					}
					return true;
				}

				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket valid transfer cannot be saved"));
				return false;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Valid transfer feature is not applicable on ticket {request.TicketId}"));
			return false;

		}

		public async Task<Unit> Handle(SetTicketAdditionalFeedBackCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.FindAsync(request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			var brand = await _brandRepository.FindAsync(ticket.BrandId);

			if (!brand.AdditionalFeedBackEnabled)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Additional feedback is not enabled for brand {brand.Id}"));
				return Unit.Value;
			}

			if (!string.IsNullOrEmpty(ticket.AdditionalFeedBack))
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Ticket {ticket.Id} additional feedback already updated."));
				return Unit.Value;
			}

			ticket.AdditionalFeedBack = request.AdditionalFeedBack;

			_ticketRepository.Update(ticket);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new TicketAdditionalFeedBackSetEvent(ticket.Id, ticket.AdvocateId, ticket.BrandId,
					ticket.IsPractice, ticket.TransportType));
				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket Additional Feedback cannot be set"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(SkipTicketAdditionalFeedBackCommand request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.FindAsync(request.TicketId);

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Ticket cannot be found."));
				return Unit.Value;
			}

			await _mediator.RaiseEvent(new TicketAdditionalFeedBackSetEvent(ticket.Id, ticket.AdvocateId, ticket.BrandId,
				ticket.IsPractice, ticket.TransportType));
			return Unit.Value;
		}

		/// <summary>
		/// The method to get the disabled the tags of the ticket brand
		/// </summary>
		/// <param name="ticket"></param>
		/// <returns></returns>
		private async Task<List<Guid>> GetDisabledTagsOfTicketBrand(Ticket ticket)
		{
			var brandTags = await _brandRepository.GetTags(ticket.BrandId);
			List<Guid> brandDisableTags = new List<Guid>();

			if(ticket.Level == TicketLevel.Regular)
			{
				brandDisableTags = brandTags.Where(x => x.L1PostClosureDisable).Select(x => x.Id).ToList();
			}
			else if (ticket.Level == TicketLevel.SuperSolver)
			{
				brandDisableTags = brandTags.Where(x => x.L2PostClosureDisable).Select(x => x.Id).ToList();
			}

			return brandDisableTags;
		}

		/// <summary>
		/// Set the ticket tags 
		/// </summary>
		/// <param name="ticketId">The ticket identifier</param>
		/// <param name="tagIds">The tags array</param>
		/// <param name="level">The ticket level</param>
		/// <param name="advocateId">The advocate identifier</param>
		/// <param name="timestamp">The timestamp</param>
		/// <returns></returns>
		private async Task SetTicketTags(Guid ticketId, Guid[] tagIds, TicketLevel level, Guid advocateId, DateTime timestamp)
		{
			var existingTags = await _ticketTagRepository.GetAllAsync(x => x.TicketId == ticketId && x.Level == level);
			var removeTags = existingTags.Where(x => !tagIds.Contains(x.TagId)).ToList();
			var updateTags = existingTags.Where(x => tagIds.Contains(x.TagId))
										.Select(x => {
											x.UserId = advocateId;
											x.CreatedDate = timestamp;
											return x;
										}).ToList();

			var newTags = tagIds.Where(x => !existingTags.Select(y => y.TagId).Contains(x))
								.Select(t => { return new TicketTag(ticketId, t, level, advocateId, timestamp);})
								.ToList();

			_ticketTagRepository.Delete(removeTags);
			_ticketTagRepository.Update(updateTags);
			await _ticketTagRepository.InsertAsync(newTags);
		}
	}
}