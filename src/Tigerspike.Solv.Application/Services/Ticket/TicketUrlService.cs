using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Services
{
	public class TicketUrlService : ITicketUrlService
	{
		private readonly IMediator _mediator;
		private readonly ITicketRepository _ticketRepository;
		private readonly ITicketService _ticketService;
		private readonly IJwtService _jwtService;
		private readonly EmailTemplatesOptions _emailTemplatesOptions;

		public TicketUrlService(
			IMediator mediator,
			IJwtService jwtService,
			ITicketRepository ticketRepository,
			ITicketService ticketService,
			IOptions<EmailTemplatesOptions> emailTemplatesOptions)
		{
			_mediator = mediator;
			_jwtService = jwtService;
			_ticketRepository = ticketRepository;
			_ticketService = ticketService;
			_emailTemplatesOptions = emailTemplatesOptions.Value;
		}

		public async Task<string> GetChatUrl(Guid ticketId, bool resume)
		{
			var token = await GetCustomerToken(ticketId);
			var probingResults = await _ticketRepository.GetProbingResults(ticketId);
			var flow = _ticketService.GetProbingEvaluation(probingResults);

			return flow.action switch
			{
				TicketFlowAction.PushbackToClient => flow.value,
				_ => string.Format(_emailTemplatesOptions.ChatUrl, ticketId, token) + (resume ? "?resume=true" : string.Empty)
			};
		}

		/// <inheritdoc/>
		public async Task<string> GetRateUrl(Guid ticketId, string culture, bool isEndChat = false)
		{
			var token = await GetCustomerToken(ticketId);
			var url = isEndChat ? string.Format(_emailTemplatesOptions.EndChatUrl, ticketId, token) : string.Format(_emailTemplatesOptions.RateUrl, ticketId, token);
			return $"{url}?culture={culture}";
		}

		/// <inheritdoc/>
		public async Task<string> GetCustomerToken(Guid ticketId)
		{
			var customerId = await _ticketRepository.GetSingleOrDefaultAsync(
				selector: x => (Guid?)x.Customer.Id,
				predicate: t => t.Id == ticketId
			);

			return customerId != null ? _jwtService.CreateTokenForTicket(ticketId, customerId.Value).Token : null;
		}


	}
}