using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Brand;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Email;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.Scheduler;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Messaging.Chat;

namespace Tigerspike.Solv.Application.Services
{
	public class TicketAutoResponseService : ITicketAutoResponseService
	{
		private readonly ILogger<TicketAutoResponseService> _logger;
		private readonly IMediator _mediator;
		private readonly IBus _bus;
		private readonly ISchedulerService _schedulerService;
		private readonly ICachedBrandAdvocateResponseConfigRepository _cachedBrandAdvocateResponseConfigRepository;
		private readonly ITemplateService _templateService;
		private readonly ITimestampService _timestampService;
		private readonly BusOptions _busOptions;

		public TicketAutoResponseService(
			ILogger<TicketAutoResponseService> logger,
			IMediator mediator,
			IBus bus,
			IOptions<BusOptions> busOptions,
			ISchedulerService schedulerService,
			ICachedBrandAdvocateResponseConfigRepository cachedBrandAdvocateResponseConfigRepository,
			ITemplateService templateService,
			ITimestampService timestampService)
		{
			_logger = logger;
			_mediator = mediator;
			_bus = bus;
			_busOptions = busOptions.Value;
			_schedulerService = schedulerService;
			_cachedBrandAdvocateResponseConfigRepository = cachedBrandAdvocateResponseConfigRepository;
			_templateService = templateService;
			_timestampService = timestampService;
		}

		public async Task SendResponses(Guid brandId, Guid ticketId, BrandAdvocateResponseType[] responseTypes, UserType userType = UserType.SolvyBot, Guid? authorId = null, BrandResponseTemplateModel model = null, int? abandonedCount = null, bool? isAutoAbandoned = null, TicketEscalationReason? escalationReason = null, TimeSpan? advanceScheduleBy = null)
		{
			var responses = await GetResponses(brandId, responseTypes, abandonedCount, isAutoAbandoned, escalationReason);
			foreach (var response in responses)
			{
				var content = _templateService.Render(response.Content, model);
				var authorUserType = response.AuthorUserType ?? userType;
				var now = _timestampService.GetUtcTimestamp();
				var scheduleTimestamp = now
					.AddSeconds(response.DelayInSeconds ?? 0)
					.Subtract(advanceScheduleBy ?? new TimeSpan());

				if (scheduleTimestamp > now)
				{
					_logger.LogDebug($"Scheduling delayed response for {response.Id} for ticket {ticketId}");
					var delayCmd = new DelayChatResponseCommand(response.Id, ticketId, authorId, response.Type, authorUserType, content, response.RelevantTo, response.ChatActionId, response.StatusOnPosting);
					await _schedulerService.ScheduleJob(delayCmd, scheduleTimestamp);
				}
				else
				{
					_logger.LogDebug($"Scheduling immediate response for {response.Id} for ticket {ticketId}");

					var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Chat}"));

					await endpoint.Send<ISendAutoChatResponseCommand>(new SendChatResponseCommand(response.Id, ticketId,
						authorId, (int?)response.Type,
						(int)authorUserType, content, response.RelevantTo, response.ChatActionId));
				}
			}
		}

		public async Task CancelResponses(Guid brandId, Guid ticketId, BrandAdvocateResponseType[] responseTypes)
		{
			var responses = await GetResponses(brandId, responseTypes);
			foreach (var response in responses)
			{
				_logger.LogDebug($"Cancelling response for {response.Id} for ticket {ticketId}");

				// obsolete code - we should no longer schedule responses directly to chat service
				await _schedulerService.CancelScheduledJob(new SendChatResponseCommand(response.Id, ticketId),
					_bus.GetQueueUri(_busOptions.Queues.Chat));

				// cancel delay response command in order not to bother processing it further
				var cmd = new DelayChatResponseCommand(response.Id, ticketId);
				await _schedulerService.CancelScheduledJob(cmd);
			}
		}

		public async Task<IEnumerable<BrandAdvocateResponseConfig>> GetResponses(Guid brandId, BrandAdvocateResponseType[] responseTypes, int? abandonedCount = null, bool? isAutoAbandoned = null, TicketEscalationReason? escalationReason = null)
		{
			var allResponses = await _cachedBrandAdvocateResponseConfigRepository.Get(brandId);
			var relevantResponses = allResponses
				.Where(x => responseTypes.Contains(x.Type)) // filter only relevant responses
				.Where(x => x.AbandonedCount == null || x.AbandonedCount == abandonedCount) // filter by abandoned count (if specified)
				.Where(x => x.IsAutoAbandoned == null || x.IsAutoAbandoned == isAutoAbandoned) // filter by is auto-abandoned (if specified)
				.Where(x => x.EscalationReason == null || x.EscalationReason == escalationReason); // filter by escalation reason (if specified)

			var topPriority = relevantResponses.Count() > 0 ? relevantResponses.Max(x => x.Priority) : 0;
			var topPriorityResponses = relevantResponses.Where(x => x.Priority == topPriority); // filter only top-proprity responses, discard the rest
			var brandSpecific = topPriorityResponses.Any(x => x.BrandId != null); // check if there are any brand specific responses or just default ones
			return brandSpecific ? topPriorityResponses.Where(x => x.BrandId != null) : topPriorityResponses; // brand specific responses should override default ones
		}

	}
}