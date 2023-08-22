using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Messaging.Notification;

namespace Tigerspike.Solv.Application.EventHandlers
{
	public class AdvocateEventHandler :
		INotificationHandler<AdvocateCreatedEvent>,
		INotificationHandler<AdvocateBrandsAssignedEvent>,
		INotificationHandler<AdvocatePractiseStartedEvent>,
		INotificationHandler<AdvocatePractiseFinishedEvent>,
		INotificationHandler<TicketCsatSetEvent>,
		INotificationHandler<TicketImportedEvent>,
		INotificationHandler<InductionItemViewedEvent>,
		INotificationHandler<AdvocateContractAgreedEvent>,
		INotificationHandler<NameChangedEvent>,
		INotificationHandler<AdvocateBrandsRemovedEvent>
	{
		private readonly IMediatorHandler _mediator;
		private readonly ITicketRepository _ticketRepository;
		private readonly IAdvocateRepository _advocateRepository;
		private readonly IBrandRepository _brandRepository;
		private readonly EmailTemplatesOptions _emailTemplatesOptions;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;

		public AdvocateEventHandler(
			IAdvocateRepository advocateRepository,
			ITicketRepository ticketRepository,
			IBrandRepository brandRepository,
			IOptions<EmailTemplatesOptions> emailTemplatesOptions,
			IMediatorHandler mediator,
			IBus bus,
			IOptions<BusOptions> busOptions)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			_advocateRepository = advocateRepository ??
								  throw new ArgumentNullException(nameof(advocateRepository));
			_ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
			_brandRepository = brandRepository ??
							   throw new ArgumentNullException(nameof(brandRepository));
			_emailTemplatesOptions = emailTemplatesOptions.Value;
			_bus = bus ?? throw new ArgumentNullException(nameof(bus));
			_busOptions = busOptions.Value;
		}

		public async Task Handle(AdvocatePractiseFinishedEvent notification, CancellationToken cancellationToken)
		{
			var ticketId = await _ticketRepository.GetPracticeTicket(notification.AdvocateId);

			if (ticketId.HasValue)
			{
				// Delete the user tickets
				await _mediator.SendCommand(new DeleteTicketCommand(ticketId.GetValueOrDefault()));
			}
		}

		public Task Handle(AdvocatePractiseStartedEvent notification, CancellationToken cancellationToken) =>
			_mediator.SendCommand(new CreateTicketCommand("John", "Smith", "john.smith@solvnow.com",
				"Welcome to practice mode", Brand.PracticeBrandId, TicketTransportType.Chat, practicingAdvocateId: notification.AdvocateId));

		public Task Handle(AdvocateCreatedEvent notification, CancellationToken cancellationToken) =>
			_mediator.SendCommand(new AssignBrandsToNewAdvocateCommand(notification.AdvocateId));

		public Task Handle(TicketCsatSetEvent notification, CancellationToken cancellationToken) =>
			_mediator.SendCommand(new UpdateAdvocateCsatCommand(notification.AdvocateId, notification.BrandId));
		public Task Handle(TicketImportedEvent notification, CancellationToken cancellationToken) =>
			notification.AdvocateId != null && notification.Csat != null ? _mediator.SendCommand(new UpdateAdvocateCsatCommand(notification.AdvocateId.Value, notification.BrandId)) : Task.CompletedTask;
		public Task Handle(InductionItemViewedEvent notification, CancellationToken cancellationToken) =>
			CheckInductionCompletion(notification.AdvocateId, notification.BrandId);
		public Task Handle(AdvocateContractAgreedEvent notification, CancellationToken cancellationToken) =>
			CheckInductionCompletion(notification.AdvocateId, notification.BrandId);

		public async Task Handle(AdvocateBrandsAssignedEvent notification, CancellationToken cancellationToken)
		{
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

			await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
			{
				MailTo = notification.AdvocateEmail,
				Subject = "New brands have been added to your profile",
				Template = EmailTemplates.AdvocateNewBrandsAssigned.ToString(),
				Model = new Dictionary<string, object>
				{
					{"ConsoleUrl", _emailTemplatesOptions.ConsoleUrl},
					{"AdvocateFirstName", notification.AdvocateFirstName},
				}
			}, cancellationToken);
		}

		public async Task Handle(NameChangedEvent notification, CancellationToken cancellationToken) =>
			await _mediator.SendCommand(new ChangeAdvocateNameCommand(notification.AdvocateId, notification.FirstName, notification.LastName));

		public async Task Handle(AdvocateBrandsRemovedEvent notification, CancellationToken cancellationToken)
		{
			var brands = await _brandRepository.GetAllAsync(b => notification.BrandIds.Contains(b.Id));
			var blockedBrands = string.Join("<br>", brands.Select(b => b.Name));
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

			await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
			{
				MailTo = notification.AdvocateEmail,
				Subject = _emailTemplatesOptions.BrandsBlockedEmailSubject,
				Template = EmailTemplates.BrandsBlocked_Email.ToString(),
				Model = new Dictionary<string, object>
				{
					{"SolverFirstName", notification.AdvocateFirstName},
					{"BlockedBrands", blockedBrands},
				}
			}, cancellationToken);
		}

		private async Task CheckInductionCompletion(Guid advocateId, Guid brandId)
		{
			var brand = await _brandRepository.FindAsync(brandId);

			var brandSectionItems = await _brandRepository.GetInductionSectionItems(brandId);

			var advocate = await _advocateRepository.GetSingleOrDefaultAsync(a => a.Id == advocateId, null,
				a => a.Include(si => si.AdvocateSectionItems), true);

			// Check if the advocate has completed all the induction items
			var inductionCompleted = brand.QuizId == null && brandSectionItems.All(brandSectionItem =>
				advocate.AdvocateSectionItems.Any(si => si.SectionItemId == brandSectionItem.Id));

			if (inductionCompleted)
			{
				await _mediator.SendCommand(new CompleteInductionCommand(advocateId, brandId));
			}
		}
	}
}