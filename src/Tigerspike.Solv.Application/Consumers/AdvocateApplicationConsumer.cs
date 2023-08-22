using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Messaging.Notification;

namespace Tigerspike.Solv.Application.Consumers
{
	public class AdvocateApplicationConsumer : IConsumer<SendProfilingReminderCommand>
	{
		private readonly ILogger<AdvocateApplicationConsumer> _logger;
		private readonly IAdvocateApplicationRepository _advocateApplicationRepository;
		private readonly EmailTemplatesOptions _emailTemplateOptions;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;

		public AdvocateApplicationConsumer(
			IAdvocateApplicationRepository advocateApplicationRepository,
			IMediatorHandler mediator,
			IBus bus,
			IOptions<BusOptions> busOptions,
			IOptions<EmailTemplatesOptions> emailTemplateOptions,
			ILogger<AdvocateApplicationConsumer> logger)
		{
			_advocateApplicationRepository = advocateApplicationRepository;
			_bus = bus ?? throw new ArgumentNullException(nameof(bus));
			_busOptions = busOptions.Value;
			_emailTemplateOptions = emailTemplateOptions.Value;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<SendProfilingReminderCommand> context)
		{
			_logger.LogDebug("Triggered profiling reminder to {email} ({advocateApplicationId})", context.Message.Email,
				context.Message.AdvocateApplicationId);
			var isProfilingFinished =
				await _advocateApplicationRepository.GetFirstOrDefaultAsync(x => (bool?) x.CompletedEmailSent,
					x => x.Id == context.Message.AdvocateApplicationId);
			if (isProfilingFinished == false)
			{
				const string emailSubject = "Don't forget to complete your skills profile!";
				var profilingUrl = string.Format(_emailTemplateOptions.AdvocateProfilingUrl,
					context.Message.AdvocateApplicationId);
				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

				await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
				{
					MailTo = context.Message.Email,
					Subject = emailSubject,
					Template = EmailTemplates.AdvocateApplicationProfiling.ToString(),
					Model = new Dictionary<string, object>
					{
						{"Subject", emailSubject},
						{"EmailLogoLocation", _emailTemplateOptions.EmailLogoLocation},
						{"ButtonUrl", profilingUrl},
					}
				});
			}
			else
			{
				_logger.LogDebug(
					"Application on {email} ({advocateApplicationId}) is no longer eligible for a profiling reminder",
					context.Message.Email, context.Message.AdvocateApplicationId);
			}
		}
	}
}