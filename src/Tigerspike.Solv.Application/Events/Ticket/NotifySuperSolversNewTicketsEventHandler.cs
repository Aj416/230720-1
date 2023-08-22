using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Messaging.Notification;

namespace Tigerspike.Solv.Application.Events.Ticket
{
	public class NotifySuperSolversNewTicketsEventHandler : INotificationHandler<TicketEscalatedEvent>
	{
		private readonly IBus _bus;
		private readonly ICachedAdvocateRepository _cachedAdvocateRepository;
		private readonly EmailTemplatesOptions _emailTemplateOptions;
		private BusOptions _busOptions;

		public NotifySuperSolversNewTicketsEventHandler(
			ICachedAdvocateRepository cachedAdvocateRepository,
			IBus bus,
			IOptions<BusOptions> busOptions,
			IOptions<EmailTemplatesOptions> emailTemplatesOptions)
		{
			_cachedAdvocateRepository = cachedAdvocateRepository ?? throw new ArgumentNullException(nameof(cachedAdvocateRepository));
			_emailTemplateOptions = emailTemplatesOptions?.Value ??
			                        throw new ArgumentNullException(nameof(emailTemplatesOptions));
			_bus = bus ?? throw new ArgumentNullException(nameof(bus));
			_busOptions = busOptions.Value;
		}

		public async Task Handle(TicketEscalatedEvent notification, CancellationToken cancellationToken)
		{
			var advocates = await _cachedAdvocateRepository.GetSuperSolvers(notification.BrandId);
			var onlineAdvocates = await _cachedAdvocateRepository.GetOnlineAdvocates(notification.BrandId);
			var offlineAdvocates = onlineAdvocates != null
				? advocates.Where(x => !onlineAdvocates.Contains(x.Id))
				: advocates;

			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

			foreach (var advocate in offlineAdvocates)
			{
				await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
				{
					MailTo = advocate.Email,
					Subject = "There are tickets waiting for you",
					Template = EmailTemplates.SuperSolverTicketEscalated.ToString(),
					Model = new Dictionary<string, object>
					{
						{"Name", advocate.FirstName},
						{"ConsoleUrl", _emailTemplateOptions.ConsoleUrl}
					},
				}, cancellationToken);
			}
		}
	}
}