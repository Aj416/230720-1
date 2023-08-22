using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Messaging.Fraud;
using Tigerspike.Solv.Services.Fraud.Application.Commands.Ticket;
using Tigerspike.Solv.Services.Fraud.Application.Commands.TicketDetection;
using Tigerspike.Solv.Services.Fraud.Application.IntegrationEvents;
using Tigerspike.Solv.Services.Fraud.Application.Services;
using Tigerspike.Solv.Services.Fraud.Models;
using Tigerspike.Solv.Services.Fraud.Rules;

namespace Tigerspike.Solv.Services.Fraud.Application.Consumers
{
	public class DetectFraudConsumer : IConsumer<IDetectFraudCommand>
	{
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;
		private readonly ILogger<DetectFraudConsumer> _logger;
		private readonly IFraudService _fraudService;
		private readonly IMediatorHandler _mediator;

		public DetectFraudConsumer(IBus bus,
			IOptions<BusOptions> busOptions,
			ILogger<DetectFraudConsumer> logger,
			IFraudService fraudService,
			IMediatorHandler mediator)
		{
			_bus = bus ??
					   throw new ArgumentNullException(nameof(bus));
			_busOptions = busOptions.Value;
			_logger = logger;
			_fraudService = fraudService;
			_mediator = mediator;
		}

		public async Task Consume(ConsumeContext<IDetectFraudCommand> context)
		{
			_logger.LogInformation($"Ticket fraud detection request received {context.Message.TicketId}");

			var msg = context.Message;
			var customerIpAddress = msg.CustomerInfoForLastDay
						.Where(x => x.TicketId == msg.TicketId && x.Key == CustomerDetail.Ip)
						.Select(x => x.Value).FirstOrDefault() ?? string.Empty;
			
			await _mediator.SendCommand(new CreateTicketCommand(msg.BrandId, msg.TicketId, msg.Level, msg.TicketStatus,
				msg.CustomerId, msg.AdvocateName, msg.BrandName, msg.Metadata, msg.CustomerFirstName, msg.CustomerLastName, 
				msg.CustomerEmail, msg.Question, customerIpAddress));

			var rules = _fraudService.GetRules(msg.TicketStatus);

			var result = new Dictionary<RuleModel, bool>();

			foreach (var rule in rules)
			{
				var ruleInstance = GetRuleInstance(
					rule.Name, msg,
					rule.Id);

				if (ruleInstance != null)
				{
					if (ruleInstance.IsValid())
					{
						result.Add(rule, await ruleInstance.IsMatchAsync());
					}
					else
					{
						_logger.LogInformation($"Fraud rule {rule.Name} is not applicable on ticket {msg.TicketId}");
					}
				}
			}

			var matches = result.Where(kvp => kvp.Value).ToList();

			if (matches.Count > 0)
			{
				await _mediator.SendCommand(new CreateTicketDetectionCommand(matches.Select(x => x.Key.Id).ToList(),
					msg.TicketId,
					msg.TicketStatus));
			}

			var rulesApplied = _fraudService.GetRulesAppliedToTicket(msg.TicketId);

			// Publish an integration event
			_logger.LogInformation($"Publishing fraud detection completed event {context.Message.TicketId}");

			await context.Publish<IFraudDetectionCompletedEvent>(
				new FraudDetectionCompletedEvent(
					msg.TicketId,
					rulesApplied.Count > 0,
					rulesApplied.Count > 0 ? rulesApplied.Select(x => (int?)x.Risk).OrderByDescending(x => x).First() : null,
					rulesApplied.Count > 0 ? rulesApplied.ToDictionary(x => x.Label, x => (int)x.Risk) : null
				));
		}

		private IRule GetRuleInstance(string ruleName, IDetectFraudCommand msg, Guid ruleId)
		{
			var strFullyQualifiedName = $"Tigerspike.Solv.Services.Fraud.Rules.{ruleName}Rule";
			var type = Type.GetType(strFullyQualifiedName);
			if (type != null)
			{
				return (IRule)Activator.CreateInstance(type, msg, ruleId);
			}

			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				type = asm.GetType(strFullyQualifiedName);
				if (type != null)
				{
					return (IRule)Activator.CreateInstance(type, msg, ruleId);
				}
			}
			return null;
		}
	}
}