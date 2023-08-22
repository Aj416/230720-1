using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack;
using MassTransit;
using MediatR;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Messaging.Notification;
using Tigerspike.Solv.Core.Email;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Localization;

namespace Tigerspike.Solv.Application.EventHandlers
{
	public class ChatEmailTransportHandler :
		INotificationHandler<TicketEscalatedEvent>,
		INotificationHandler<TicketSolvedEvent>,
		INotificationHandler<TicketClosedEvent>
	{
		private readonly EmailTemplatesOptions _emailTemplatesOptions;
		private readonly ITicketService _ticketService;
		private readonly ITicketUrlService _ticketUrlService;
		private readonly IMediatorHandler _mediator;
		private readonly ITicketRepository _ticketRepository;
		private readonly ICachedTicketRepository _cachedTicketRepository;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;
		private readonly ILocalizationProviderFactory _localizationProviderFactory;
		private readonly ITemplateService _templateService;
		private readonly IShortUrlService _shortUrlService;
		private readonly IChatService _chatService;

		public ChatEmailTransportHandler(
			ILocalizationProviderFactory localizationProviderFactory,
			IChatService chatService,
			ITemplateService templateService,
			IShortUrlService shortUrlService,
			Microsoft.Extensions.Options.IOptions<EmailTemplatesOptions> emailTemplatesOptions,
			ITicketService ticketService,
			ITicketUrlService ticketUrlService,
			ITicketRepository ticketRepository,
			ICachedTicketRepository cachedTicketRepository,
			IMediatorHandler mediator,
			IBus bus, Microsoft.Extensions.Options.IOptions<BusOptions> busOptions)
		{
			_localizationProviderFactory = localizationProviderFactory;
			_chatService = chatService;
			_templateService = templateService;
			_shortUrlService = shortUrlService;
			_emailTemplatesOptions = emailTemplatesOptions.Value;
			_ticketService = ticketService;
			_ticketUrlService = ticketUrlService;
			_ticketRepository = ticketRepository;
			_mediator = mediator;
			_bus = bus;
			_cachedTicketRepository = cachedTicketRepository;
			_busOptions = busOptions.Value;
		}

		public async Task Handle(TicketEscalatedEvent notification, CancellationToken cancellationToken)
		{
			var transportModel = await _cachedTicketRepository.GetTransportModel(notification.TicketId, notification.AdvocateId);

			if (transportModel?.TransportType == TicketTransportType.Email)
			{
				var localizationProvider = _localizationProviderFactory.GetLocalizationProvider(transportModel.Culture);
				var templateModel = new
				{
					Question = transportModel.Question.Truncate(30, true),
					transportModel.BrandName
				};
				var emailSubject =
					_templateService.Render(localizationProvider.Resources.Emails.TicketEscalated.Subject, templateModel);
				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));
				var sender = _templateService.Render(localizationProvider.Resources.Emails.Sender, templateModel);
				await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
				{
					Culture = transportModel.Culture,
					MailTo = transportModel.CustomerEmail,
					Subject = emailSubject,
					Template = EmailTemplates.TicketEscalated.ToString(),
					Model = transportModel.ToObjectDictionary(),
					SenderName = sender
				}, cancellationToken);
			}
			else if (transportModel?.TransportType == TicketTransportType.Messenger)
			{
				var message =
					$"Your question has been escalated, someone else will be in touch soon.";

				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

				await endpoint.Send<ISendMessengerMessageCommand>(new
				{
					ConversationId = notification.ThreadId,
					Text = message
				}, cancellationToken);
			}
		}

		public async Task Handle(TicketSolvedEvent notification, CancellationToken cancellationToken)
		{
			var transportModel = await _cachedTicketRepository.GetTransportModel(notification.TicketId, notification.AdvocateId);

			if (transportModel?.TransportType == TicketTransportType.Email)
			{
				var localizationProvider = _localizationProviderFactory.GetLocalizationProvider(transportModel.Culture);
				var chatUrl = await _ticketUrlService.GetChatUrl(notification.TicketId, true);
				var rateUrl = await _ticketUrlService.GetRateUrl(notification.TicketId, transportModel.Culture);
				var model = new TicketSolvedNotificationModel
				{
					Culture = localizationProvider.CultureInfo,
					Number = transportModel.Number,
					BrandLogoUrl = transportModel.BrandLogoUrl,
					CustomerFirstName = transportModel.CustomerFirstName,
					AdvocateFirstName = transportModel.AdvocateFirstName,
					ChatUrl = chatUrl,
					RateUrl = rateUrl,
					Question = transportModel.Question,
					QuestionSummary = transportModel.Question.Truncate(15, true),
					ClosingTime = transportModel.ClosingTime,
				};

				var templateModel = new
				{
					AdvocateFirstName = transportModel.AdvocateFirstName,
					BrandName = transportModel.BrandName,
					Question = model.QuestionSummary,
					Number = transportModel.Number,
				};
				var emailSubject = _templateService.Render(localizationProvider.Resources.Emails.TicketSolved_Email.Subject, templateModel);
				var sender = _templateService.Render(localizationProvider.Resources.Emails.Sender, templateModel);
				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

				await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
				{
					Culture = transportModel.Culture,
					ReplyToTicket = true,
					MailTo = transportModel.CustomerEmail,
					Subject = emailSubject,
					Template = EmailTemplates.TicketSolved_Email.ToString(),
					Model = model.ToObjectDictionary(),
					SenderName = sender,
				}, cancellationToken);
			}
			else if (transportModel?.TransportType == TicketTransportType.Messenger)
			{
				var message = $"{transportModel.AdvocateFirstName} is requesting to close the ticket. Reply with 'yes' to confirm or anything else to continue chatting";
				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

				await endpoint.Send<ISendMessengerMessageCommand>(new
				{
					ConversationId = notification.ThreadId,
					Text = message
				}, cancellationToken);
			}
		}

		public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken)
		{
			var transportModel = await _cachedTicketRepository.GetTransportModel(notification.TicketId, notification.AdvocateId);

			if (transportModel?.TransportType == TicketTransportType.Email)
			{
				var transcript = await _chatService.GetTranscript(notification.TicketId);

				if (notification.ClosedBy == ClosedBy.System || notification.ClosedBy == ClosedBy.CutOff)
				{
					// if the system closed it automatically, we send an email with a link to Close/CSAT page.
					transportModel.ChatUrl = await _ticketUrlService.GetRateUrl(notification.TicketId, transportModel.Culture);
					var localizationProvider = _localizationProviderFactory.GetLocalizationProvider(transportModel.Culture);
					var templateModel = new
					{
						AdvocateFirstName = transportModel.AdvocateFirstName,
						Question = transportModel.Question.Truncate(30, true),
						BrandName = transportModel.BrandName,
					};
					var emailSubject = _templateService.Render(localizationProvider.Resources.Emails.TicketClosed_Customer_Email.Subject, templateModel);
					var sender = _templateService.Render(localizationProvider.Resources.Emails.Sender, templateModel);
					var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

					await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
					{
						Culture = transportModel.Culture,
						MailTo = transportModel.CustomerEmail,
						Subject = emailSubject,
						Template = EmailTemplates.TicketClosed_System_Email.ToString(),
						Model = transportModel.ToObjectDictionary(),
						SenderName = sender,
						Attachment = new EmailAttachment
						{
							ContentType = MediaTypeNames.Text.Plain,
							Filename = $"ticket-transcript-{notification.TicketId}.txt",
							Content = transcript
						}
					}, cancellationToken);

				}
				else if (notification.ClosedBy == ClosedBy.Customer || notification.ClosedBy == ClosedBy.EndChat)
				{
					// if the customer closed it, we send an email (similar to Chat ticket closed) with transcript only.
					(var question, var createdDate, var brandName, var brandLogo, var advocateFirstName, var customerEmail) =
						await _ticketRepository.GetSingleOrDefaultAsync(predicate: x => x.Id == notification.TicketId,
						selector: x => Tuple.Create(x.Question, x.CreatedDate, x.Brand.Name, x.Brand.Logo, x.Advocate.User.FirstName, x.Customer.Email).ToValueTuple());

					var localizationProvider = _localizationProviderFactory.GetLocalizationProvider(transportModel.Culture);
					var templateModel = new
					{
						Question = question.Truncate(30, true),
						BrandName = brandName
					};
					var emailSubject = _templateService.Render(localizationProvider.Resources.Emails.TicketClosed_Customer_Email.Subject, templateModel);
					var sender = _templateService.Render(localizationProvider.Resources.Emails.Sender, templateModel);
					var conversation = await _chatService.GetTranscript(notification.TicketId);
					var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

					await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
					{
						Culture = transportModel.Culture,
						MailTo = customerEmail,
						Subject = emailSubject,
						Template = EmailTemplates.TicketClosed_Customer_Email.ToString(),
						Model = new Dictionary<string, object>
						{
							{"question", question},
							{"CreatedDate", createdDate.ToString("MMM dd, hh:mm", localizationProvider.CultureInfo)},
							{"BrandName", brandName},
							{"BrandLogoUrl", brandLogo},
							{"advocateFirstName", advocateFirstName},
						},
						SenderName = sender,
						Attachment = new EmailAttachment
						{
							ContentType = MediaTypeNames.Text.Plain,
							Filename = $"ticket-transcript-{notification.TicketId}.txt",
							Content = conversation,
						}
					}, cancellationToken);
				}
				else
				{
					throw new Exception($"Unidentified value for ClosedBy {notification.ClosedBy}");
				}
			}
			else if (transportModel?.TransportType == TicketTransportType.Messenger)
			{
				var rateUrl = await _ticketUrlService.GetRateUrl(notification.TicketId, transportModel.Culture);

				// shorten url before sending it
				var shortUrl = await _shortUrlService.GetShortUrl(rateUrl);

				var message =
					$"How would you rate the {transportModel.BrandName} support you received from {transportModel.AdvocateFirstName}? Click here to let us know {shortUrl}";

				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

				await endpoint.Send<ISendMessengerMessageCommand>(new
				{
					ConversationId = notification.ThreadId,
					Text = message
				}, cancellationToken);
			}
		}
	}
}