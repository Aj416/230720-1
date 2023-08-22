using System;
using System.Threading.Tasks;
using MassTransit;
using ServiceStack;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Core.Email;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Localization;
using Tigerspike.Solv.Messaging.Chat;
using Tigerspike.Solv.Messaging.Notification;

namespace Tigerspike.Solv.Application.Consumers.Chat
{
	public class NotifyWhenEmailTransportConsumer : IConsumer<IChatMessageAddedEvent>
	{
		private readonly ITicketUrlService _ticketUrlService;
		private readonly ICachedTicketRepository _cachedTicketRepository;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;
		private readonly ILocalizationProviderFactory _localizationProviderFactory;
		private readonly ITemplateService _templateService;

		public NotifyWhenEmailTransportConsumer(
			ILocalizationProviderFactory localizationProviderFactory,
			ITemplateService templateService,
			Microsoft.Extensions.Options.IOptions<EmailTemplatesOptions> emailTemplatesOptions,
			ITicketUrlService ticketUrlService,
			ICachedTicketRepository cachedTicketRepository,
			IBus bus, Microsoft.Extensions.Options.IOptions<BusOptions> busOptions)
		{
			_localizationProviderFactory = localizationProviderFactory;
			_templateService = templateService;
			_ticketUrlService = ticketUrlService;
			_bus = bus;
			_cachedTicketRepository = cachedTicketRepository;
			_busOptions = busOptions.Value;
		}

		public async Task Consume(ConsumeContext<IChatMessageAddedEvent> context)
		{
			var notification = context.Message;

			if (notification.SenderType == (int)UserType.Advocate || notification.SenderType == (int)UserType.SuperSolver)
			{
				var transportModel = await _cachedTicketRepository.GetTransportModel(notification.ConversationId, notification.AuthorId);

				if (transportModel?.TransportType == TicketTransportType.Email)
				{
					var localizationProvider = _localizationProviderFactory.GetLocalizationProvider(transportModel.Culture);
					transportModel.RateUrl = await _ticketUrlService.GetRateUrl(notification.ConversationId, transportModel.Culture, transportModel.EndChatEnabled);
					transportModel.Message = notification.Message;
					var templateModel = new
					{
						BrandName = transportModel.BrandName,
						Question = transportModel.Question.Truncate(15, true),
						Number = transportModel.Number,
					};
					var emailSubject = _templateService.Render(localizationProvider.Resources.Emails.AdvocateRepliedInEmail.Subject, templateModel);
					var sender = _templateService.Render(localizationProvider.Resources.Emails.Sender, templateModel);

					var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

					await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
					{
						Culture = transportModel.Culture,
						ReplyToTicket = true,
						MailTo = transportModel.CustomerEmail,
						Subject = emailSubject,
						Template = EmailTemplates.AdvocateRepliedInEmail.ToString(),
						Model = transportModel.ToObjectDictionary(),
						SenderName = sender,
					});
				}
				else if (transportModel?.TransportType == TicketTransportType.Messenger && notification.Action != null)
				{
					var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

					await endpoint.Send<ISendMessengerMessageCommand>(new
					{
						ConversationId = notification.ThreadId,
						Text = notification.Message
					});
				}
			}
		}
	}
}