using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Enum;
using Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces;
using Tigerspike.Solv.Services.Fraud.Models;

namespace Tigerspike.Solv.Services.Fraud.Application.Services
{
	/// <summary>
	/// IFraudService interface.
	/// </summary>
	public class FraudService : IFraudService
	{
		private readonly ILogger<FraudService> _logger;
		private readonly IMediatorHandler _mediator;
		private readonly ITicketRepository _ticketRepository;
		private readonly ITicketDetectionRepository _ticketDetectionRepository;
		private readonly IRuleRepository _ruleRepository;
		private readonly IMapper _mapper;

		public FraudService(
			ITicketRepository ticketRepository,
			IRuleRepository ruleRepository,
			ILogger<FraudService> logger,
			IMediatorHandler mediator,
			IMapper mapper,
			ITicketDetectionRepository ticketDetectionRepository
		)
		{
			_ticketRepository = ticketRepository;
			_ruleRepository = ruleRepository;
			_logger = logger;
			_mediator = mediator;
			_mapper = mapper;
			_ticketDetectionRepository = ticketDetectionRepository;
		}

		/// <inheritdoc />
		public List<RuleModel> GetRules(int status)
		{
			var rules = _ruleRepository.GetRules(status);
			return _mapper.Map<List<RuleModel>>(rules);
		}

		private List<RuleModel> GetRules()
		{
			var openRules = _ruleRepository.GetRules((int)TicketStatus.New);
			var inProgressRules = _ruleRepository.GetRules((int)TicketStatus.Assigned);
			var solvedRules = _ruleRepository.GetRules((int)TicketStatus.Solved);
			var closedRules = _ruleRepository.GetRules((int)TicketStatus.Closed);

			var rules = new List<Rule>();
			rules.AddRange(openRules);
			rules.AddRange(inProgressRules);
			rules.AddRange(solvedRules);
			rules.AddRange(closedRules);

			return _mapper.Map<List<RuleModel>>(rules);
		}

		/// <inheritdoc />
		public List<RuleModel> GetRulesAppliedToTicket(Guid ticketId)
		{
			var rules = GetRules();
			var detection = _ticketDetectionRepository.GetTicketDetection(ticketId.ToString());
			if (detection != null)
			{
				var appliedRules = _mapper.Map<List<Guid>>(detection.Rules);
				return rules.Where(x => appliedRules.Contains(x.Id)).ToList();
			}

			return new List<RuleModel>();
		}

		/// <inheritdoc />
		public TicketModel GetTicketDetails(Guid ticketId)
		{
			var ticket = _mapper.Map<TicketModel>(_ticketRepository.GetTicket(ticketId.ToString()));
			if (ticket != null)
			{
				ticket.Rules = GetRulesAppliedToTicket(ticketId);
			}

			return ticket;
		}
	}
}