using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.Scheduler;
using Tigerspike.Solv.Messaging.Notification;

namespace Tigerspike.Solv.Application.EventHandlers
{
	public class AdvocateApplicationEventHandler :
		INotificationHandler<AdvocateApplicationCompletedEvent>,
		INotificationHandler<AdvocateApplicationCreatedEvent>,
		INotificationHandler<NameChangedEvent>,
		INotificationHandler<AdvocateApplicationProfileSubmittedEvent>
	{
		private readonly ITimestampService _timestampService;
		private readonly ISchedulerService _schedulerService;
		private readonly EmailTemplatesOptions _emailTemplateOptions;
		private readonly IMediatorHandler _mediator;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;

		public AdvocateApplicationEventHandler(
			IBus bus,
			IOptions<EmailTemplatesOptions> emailTemplateOptions,
			IOptions<BusOptions> busOptions,
			ITimestampService timestampService,
			ISchedulerService schedulerService,
			IMediatorHandler mediator
		)
		{
			_timestampService = timestampService;
			_schedulerService = schedulerService;
			_emailTemplateOptions = emailTemplateOptions?.Value ??
									throw new ArgumentNullException(nameof(emailTemplateOptions));
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			_bus = bus ?? throw new ArgumentNullException(nameof(bus));
			_busOptions = busOptions.Value;
		}

		public async Task Handle(AdvocateApplicationCompletedEvent notification, CancellationToken cancellationToken)
		{
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

			await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
			{
				MailTo = notification.Email,
				Subject = _emailTemplateOptions.AdvocateApplicationCreatedEmailSubject,
				Template = EmailTemplates.AdvocateApplicationCompleted.ToString(),
				Model = new Dictionary<string, object>
				{
					{"AdvocateApplicationCreatedEmailSubject", _emailTemplateOptions.AdvocateApplicationCreatedEmailSubject},
					{"EmailLogoLocation", _emailTemplateOptions.EmailLogoLocation}
				}
			}, cancellationToken);

			await _mediator.SendCommand(
				new SetAdvocateApplicationCompletedEmailSentCommand(notification.ApplicationId));
		}

		public async Task Handle(AdvocateApplicationCreatedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.InternalAgent == false)
			{
				var cmd = new SendProfilingReminderCommand(notification.ApplicationId, notification.Email);
				var deliveryTimestamp = _timestampService.GetUtcTimestamp() +
										TimeSpan.FromSeconds(_emailTemplateOptions.ProfilingReminderDelaySeconds);
				await _schedulerService.ScheduleJob(cmd, deliveryTimestamp);
			}
		}

		public async Task Handle(NameChangedEvent notification, CancellationToken cancellationToken) =>
			await _mediator.SendCommand(new ChangeAdvocateApplicationNameCommand(notification.AdvocateId, notification.FirstName, notification.LastName));

		public async Task Handle(AdvocateApplicationProfileSubmittedEvent notification, CancellationToken cancellationToken) =>
			await _mediator.SendCommand(new UpdateAdvocateProfileStatusCommand(notification.AdvocateId, notification.ProfileQuestionId));
	}
}