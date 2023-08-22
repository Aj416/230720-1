using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Redis;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.SignalR;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Core.Email;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Messaging.Chat;
using Tigerspike.Solv.Messaging.Invoicing;
using Tigerspike.Solv.Messaging.Notification;
using static Tigerspike.Solv.Core.Constants.CacheKeys;

namespace Tigerspike.Solv.Application.Consumers
{
	public class TicketConsumer :
		IConsumer<StartTicketTimeoutEscalation>,
		IConsumer<CancelTicketReservationCommand>,
		IConsumer<CloseTicketWhenNoResponseCommand>,
		IConsumer<SendChatReminderCommand>,
		IConsumer<SendCloseTicketReminderCommand>,
		IConsumer<DelayChatResponseCommand>,
		IConsumer<IFetchClientInvoicingAmountCommand>,
		IConsumer<ISetTicketsClientInvoiceIdCommand>,
		IConsumer<IFetchAdvocatesToInvoiceCommand>,
		IConsumer<IFetchTicketsForAdvocateInvoiceCommand>,
		IConsumer<ISetTicketsAdvocateInvoiceIdCommand>,
		IConsumer<IFetchTicketInfoCommand>,
		IConsumer<IFetchTicketsInfoForAdvocateInvoiceCommand>,
		IConsumer<IUpdateAdvocateStatisticsWebhookCommand>
	{
		private readonly ILogger<TicketConsumer> _logger;
		private readonly IHubContext<TicketHub> _ticketHub;
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly ITicketRepository _ticketRepository;
		private readonly ICachedTicketRepository _cachedTicketRepository;
		private readonly ITicketService _ticketService;
		private readonly ITicketUrlService _ticketUrlService;
		private readonly IMediatorHandler _mediator;
		private readonly ITimestampService _timestampService;
		private readonly EmailTemplatesOptions _emailTemplatesOptions;
		private readonly ITemplateService _templateService;
		private readonly ITicketEscalationConfigRepository _ticketEscalationConfigRepository;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;

		public TicketConsumer(
			ITicketEscalationConfigRepository ticketEscalationConfigRepository,
			ITicketRepository ticketRepository,
			ICachedTicketRepository cachedTicketRepository,
			ITicketService ticketService,
			ITicketUrlService ticketUrlService,
			IMediatorHandler mediator,
			IBus bus,
			Microsoft.Extensions.Options.IOptions<BusOptions> busOptions,
			ITimestampService timestampService,
			Microsoft.Extensions.Options.IOptions<EmailTemplatesOptions> emailTemplatesOptions,
			ITemplateService templateService,
			ILogger<TicketConsumer> logger,
			IHubContext<TicketHub> ticketHub,
			IRedisClientsManager redisClientsManager)
		{
			_ticketEscalationConfigRepository = ticketEscalationConfigRepository;
			_ticketRepository = ticketRepository;
			_ticketService = ticketService;
			_ticketUrlService = ticketUrlService;
			_mediator = mediator;
			_bus = bus ?? throw new ArgumentNullException(nameof(bus));
			_busOptions = busOptions.Value;
			_timestampService = timestampService;
			_emailTemplatesOptions = emailTemplatesOptions.Value;
			_templateService = templateService;
			_logger = logger;
			_ticketHub = ticketHub;
			_redisClientsManager = redisClientsManager;
			_cachedTicketRepository = cachedTicketRepository;
		}


		public async Task Consume(ConsumeContext<CancelTicketReservationCommand> context)
		{
			_logger.LogInformation($"Ticket expired {context.Message.TicketId}");

			var originalTicket = await _ticketRepository.GetFirstOrDefaultAsync(
				selector: x => new
				{
					x.Id,
					x.ReferenceId,
					x.SourceId,
					x.BrandId,
					x.AdvocateId,
					x.Status,
					AdvocateFirstName = x.Advocate.User.FirstName,
					x.Source,
					x.Level,
					x.RejectionCount,
					x.CreatedDate,
					CustomerId = x.Customer.Id
				},
				predicate: x => x.Id == context.Message.TicketId
			);

			if (originalTicket != null)
			{
				if (originalTicket.Status == TicketStatusEnum.Reserved &&
					originalTicket.AdvocateId == context.Message.AdvocateId)
				{
					_logger.LogInformation($"Cancelling reservation {context.Message.TicketId}");

					var expiredTicket =
						await _mediator.SendCommand(new ExpireTicketReservationCommand(originalTicket.Id));

					var escalationConfig =
						await _ticketEscalationConfigRepository.Get(originalTicket.BrandId, originalTicket.SourceId);

					if (_ticketService.IsEscalationRejectionThresholdReached(expiredTicket, escalationConfig))
					{
						await _mediator.SendCommand(new EscalateTicketCommand(originalTicket.Id,
							TicketEscalationReason.RejectionCountExceeded));
					}
					else if (_ticketService.IsEscalationTimeoutReached(expiredTicket, escalationConfig))
					{
						await _mediator.SendCommand(new EscalateTicketCommand(context.Message.TicketId,
							TicketEscalationReason.OpenTimeExceeded));
					}

					await _mediator.RaiseEvent(new TicketRejectedEvent(originalTicket.Id, originalTicket.BrandId, originalTicket.Level,
						originalTicket.AdvocateId.Value, originalTicket.Source?.Id, originalTicket.CreatedDate,
						originalTicket.RejectionCount, originalTicket.CustomerId));
				}
				else
				{
					_logger.LogDebug($"Ticket {context.Message.TicketId} is no more valid for cancelling reservation");
				}
			}
			else
			{
				_logger.LogDebug($"Ticket {context.Message.TicketId} was not found");
			}
		}

		public async Task Consume(ConsumeContext<CloseTicketWhenNoResponseCommand> context)
		{
			_logger.LogInformation($"Ticket should be closed {context.Message.TicketId}");

			var ticket = await _ticketRepository.GetFullTicket(t => t.Id == context.Message.TicketId);

			if (ticket != null)
			{
				if (_ticketService.ShouldTicketBeClosed(ticket))
				{
					_logger.LogDebug($"Closing ticket {context.Message.TicketId}");
					await _ticketService.Close(ticket.Id, ClosedBy.System);
				}
				else
				{
					_logger.LogDebug($"Ticket {context.Message.TicketId} is no more valid for closing ({ticket.Status}, {ticket.ModifiedDate})");
				}
			}
			else
			{
				_logger.LogDebug($"Ticket {context.Message.TicketId} was not found");
			}
		}

		public async Task Consume(ConsumeContext<StartTicketTimeoutEscalation> context)
		{
			_logger.LogInformation("Ticket {ticketId} triggered for escalation", context.Message.TicketId);

			var originalTicket = await _ticketRepository.GetFirstOrDefaultAsync(
				selector: x => new
				{
					x.IsPractice,
					x.BrandId,
					x.ReferenceId,
					x.Source,
					x.Question,
					x.Status,
					x.Level,
					AdvocateFirstName = x.Advocate.User.FirstName
				},
				predicate: x => x.Id == context.Message.TicketId
			);

			if (originalTicket != null)
			{
				if (originalTicket.Status == TicketStatusEnum.New && originalTicket.Level == TicketLevel.Regular)
				{
					_logger.LogDebug("Ticket {ticketId} is being escalated due to {reason}", context.Message.TicketId,
						TicketEscalationReason.OpenTimeExceeded);

					await _mediator.SendCommand(new EscalateTicketCommand(context.Message.TicketId,
						TicketEscalationReason.OpenTimeExceeded));
				}
				else
				{
					_logger.LogDebug("Ticket {ticketId} is no longer eligble for escalation", context.Message.TicketId);
				}
			}
			else
			{
				_logger.LogDebug("Ticket {ticketId} is no longer found for escalation", context.Message.TicketId);
			}
		}

		public async Task Consume(ConsumeContext<SendChatReminderCommand> context)
		{
			_logger.LogInformation("Sending reminder email for ticket {ticketId}", context.Message.TicketId);

			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

			await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
			{
				MailTo = context.Message.CustomerEmail,
				Subject = context.Message.Subject,
				Template = EmailTemplates.AdvocateRepliedInChat.ToString(),
				Model = context.Message.ToObjectDictionary(),
				SenderName = $"{context.Message.BrandName} Support"
			});
		}

		public async Task Consume(ConsumeContext<SendCloseTicketReminderCommand> context)
		{
			var (ticketId, advocateId) = (context.Message.TicketId, context.Message.AdvocateId);
			var transportModel = await _cachedTicketRepository.GetTransportModel(ticketId, advocateId);
			var chatUrl = await _ticketUrlService.GetChatUrl(ticketId, true);
			var rateUrl = await _ticketUrlService.GetRateUrl(ticketId, transportModel.Culture);
			var model = new TicketSolvedNotificationModel
			{
				Number = transportModel.Number,
				BrandLogoUrl = transportModel.BrandLogoUrl,
				CustomerFirstName = transportModel.CustomerFirstName,
				AdvocateFirstName = transportModel.AdvocateFirstName,
				ChatUrl = chatUrl,
				RateUrl = rateUrl,
				Question = transportModel.Question,
				QuestionSummary = transportModel.Question.Truncate(30, true),
			};

			model.Subject = _templateService.Render(context.Message.Subject, model);
			model.Header = _templateService.Render(context.Message.Header, model);
			model.Body = _templateService.Render(context.Message.Body, model);

			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

			await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
			{
				ReplyToTicket = true,
				MailTo = transportModel.CustomerEmail,
				Subject = model.Subject,
				Template = EmailTemplates.TicketSolved_Chat.ToString(),
				Model = model.ToObjectDictionary(),
				SenderName = $"{transportModel.BrandName} Support"
			});

			await _mediator.RaiseEvent(new TicketChaserEmailSentEvent(ticketId));
		}

		public async Task Consume(ConsumeContext<DelayChatResponseCommand> context)
		{
			var msg = context.Message;
			_logger.LogDebug($"Checking DelayChatResponseCommand for {msg.ResponseId} / ticket {msg.TicketId}");


			// check if chat response is still valid
			if (msg.StatusOnPosting != null)
			{
				// get current ticket status
				var ticket = await _ticketRepository.GetFirstOrDefaultAsync(
					selector: x => new { x.Status },
					predicate: x => x.Id == msg.TicketId
				);

				if (ticket == null)
				{
					_logger.LogError($"Message {msg?.ResponseId} for ticket {msg?.TicketId} is no longer valid (ticket does not exist)");
					return;
				}

				if (ticket.Status != msg.StatusOnPosting)
				{
					_logger.LogDebug($"Message {msg?.ResponseId} for ticket {msg?.TicketId} is no longer valid (required status: {msg.StatusOnPosting} vs {ticket.Status} on the ticket)");
					return;
				}
			}

			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Chat}"));
			await endpoint.Send<ISendAutoChatResponseCommand>(new SendChatResponseCommand(msg.ResponseId, msg.TicketId,
				msg.AuthorId, msg.ResponseType, msg.SenderType, msg.Content, msg.RelevantTo, msg.ActionId));
		}

		public async Task Consume(ConsumeContext<IFetchClientInvoicingAmountCommand> context)
		{
			var msg = context.Message;
			var (priceTotal, feeTotal, ticketsCount) = await _ticketRepository.GetClientInvoicingAmounts(msg.From, msg.To, msg.BrandId);

			await context.RespondAsync<IFetchClientInvoicingAmountResult>(new
			{
				IsSuccess = true,
				PriceTotal = priceTotal,
				FeeTotal = feeTotal,
				TicketCount = ticketsCount
			});
		}

		public async Task Consume(ConsumeContext<ISetTicketsClientInvoiceIdCommand> context)
		{
			var msg = context.Message;
			_logger.LogDebug($"Setting InvoiceId { msg.ClientInvoiceId} for brand {msg.BrandId} specific tickets");

			// relate tickets involved in calculations with created invoice
			await _ticketRepository.SetTicketsClientInvoiceId(msg.ClientInvoiceId, msg.BrandId, msg.From, msg.To);
		}

		public async Task Consume(ConsumeContext<IFetchAdvocatesToInvoiceCommand> context)
		{
			_logger.LogDebug("Consuming IFetchAdvocatesToInvoiceCommand to fetch advocate ids with valid tickets for invoicing");

			var msg = context.Message;
			var advocatesWithTickets = await _ticketRepository.GetAdvocatesToInvoice(msg.From, msg.To);

			await context.RespondAsync<IFetchAdvocateIdsForInvoicingResult>(new
			{
				IsSuccess = true,
				AdvocateIds = advocatesWithTickets
			});
		}

		public async Task Consume(ConsumeContext<IFetchTicketsForAdvocateInvoiceCommand> context)
		{
			_logger.LogDebug("Consuming IFetchTicketsForAdvocateInvoiceCommand to fetch tickets by brand for invoicing");
			var msg = context.Message;
			var ticketsByBrand = await _ticketRepository.GetTicketsWithBrandNameForAdvocateInvoice(msg.From, msg.To, msg.AdvocateId);

			await context.RespondAsync<IFetchTicketsForAdvocateInvoiceResult>(new
			{
				IsSuccess = true,
				TicketsByBrand = ticketsByBrand
			});
		}

		public async Task Consume(ConsumeContext<ISetTicketsAdvocateInvoiceIdCommand> context)
		{
			var msg = context.Message;
			_logger.LogDebug($"Setting InvoiceId { msg.AdvocateInvoiceId} for tickets assigned to advocate {msg.AdvocateId}");

			// relate tickets involved in calculations with created invoice
			await _ticketRepository.SetTicketsAdvocateInvoiceId(msg.AdvocateInvoiceId, msg.AdvocateId, msg.From, msg.To);
		}

		public async Task Consume(ConsumeContext<IFetchTicketInfoCommand> context)
		{
			var msg = context.Message;
			_logger.LogDebug($"Fetching ticket's info for client invoice model");

			var tickets = new List<Ticket>();

			if (msg.ClientInvoiceId.HasValue)
			{
				tickets.AddRange(await _ticketRepository.GetAllAsync(
				include: inc => inc.Include(t => t.Brand),
				predicate: src => src.ClientInvoiceId == msg.ClientInvoiceId.Value));
			}
			else if (msg.AdvocateInvoiceId.HasValue)
			{
				tickets.AddRange(await _ticketRepository.GetAllAsync(
				include: inc => inc.Include(t => t.Brand),
				predicate: src => src.AdvocateInvoiceId == msg.AdvocateInvoiceId.Value));
			}

			await context.RespondAsync<IFetchTicketInfoResult>(new
			{
				IsSuccess = true,
				TicketInfo = tickets.Select(t => new
				{
					t.Id,
					t.Price,
					t.Fee,
					t.Total,
					t.CreatedDate,
					Brand = new
					{
						Id = t.BrandId,
						t.Brand.Name
					}
				})
			});
		}

		public async Task Consume(ConsumeContext<IFetchTicketsInfoForAdvocateInvoiceCommand> context)
		{
			var msg = context.Message;
			_logger.LogInformation($"Consuming IFetchTicketsInfoForAdvocateInvoiceCommand to get tickets info for advocate invoice {msg.AdvocateInvoiceId}");

			// Get the list of the tickets information to be listed in the PayPal order.
			var invoicedTickets = await _ticketRepository.GetTicketsInfoForAdvocateInvoice(msg.AdvocateInvoiceId);

			await context.RespondAsync<IFetchTicketsInfoForAdvocateInvoiceResult>(new
			{
				IsSuccess = true,
				InvoicedTickets = invoicedTickets
			});
		}

		public async Task Consume(ConsumeContext<IUpdateAdvocateStatisticsWebhookCommand> context)
		{
			var msg = context.Message;
			_logger.LogInformation($"Updating advocate statistics for advocate {msg.AdvocateId}");

			if (msg.AdvocateId.HasValue)
			{
				using var client = _redisClientsManager.GetClient();

				// invalidate statistics changed due to making a payment for an advocate.
				client.Remove(PreviousWeekStatisticsPeriodKey(msg.AdvocateId.Value));

				// Notify the advocate that the statistics has changed.
				await _ticketHub.Clients.User(msg.AdvocateId.ToString()).SendAsync(TicketHubConstants.ADVOCATE_STATISTICS_UPDATED);
			}
		}
	}
}
