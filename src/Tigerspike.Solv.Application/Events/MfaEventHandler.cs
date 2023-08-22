using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Domain.Events.User;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Localization;
using Tigerspike.Solv.Messaging.Notification;

namespace Tigerspike.Solv.Application.EventHandlers
{
	public class MfaEventHandler :
		INotificationHandler<MfaResetEvent>
	{
		private readonly ILocalizationProviderFactory _localizationProviderFactory;
		private readonly IBus _bus;
		private readonly EmailTemplatesOptions _emailTemplatesOptions;
		private readonly BusOptions _busOptions;

		public MfaEventHandler(
			ILocalizationProviderFactory _localizationProviderFactory,
			IBus bus,
			IOptions<BusOptions> busOptions,
			IOptions<EmailTemplatesOptions> emailTemplatesOptions)
		{
			this._localizationProviderFactory = _localizationProviderFactory;
			_bus = bus;
			_emailTemplatesOptions = emailTemplatesOptions.Value;
			_busOptions = busOptions.Value;
		}

		public async Task Handle(MfaResetEvent notification, CancellationToken cancellationToken)
		{
			var culture = string.Empty; // for now this email is not localized
			var localizationProvider = _localizationProviderFactory.GetLocalizationProvider(culture);
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

			await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
			{
				Culture = culture,
				MailTo = notification.UserEmail,
				Subject = localizationProvider.Resources.Emails.MfaReset.Subject,
				Template = EmailTemplates.MfaReset.ToString(),
				Model = new Dictionary<string, object>
				{
					{ "ConsoleUrl", _emailTemplatesOptions.ConsoleUrl },
					{ "BrandLogoUrl", _emailTemplatesOptions.EmailLogoLocation },
				}
			}, cancellationToken);
		}
	}
}
