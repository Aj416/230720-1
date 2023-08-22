using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Refit;
using Tigerspike.Solv.Application.Models.Fraud;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Api.Services
{
	/// <summary>
	///
	/// </summary>
	public class FraudService : IFraudService
	{
		private readonly IFraudApi _client;
		private readonly IMediatorHandler _mediator;

		/// <summary>
		/// FraudService constructor.
		/// </summary>
		public FraudService(
			IOptions<ServiceUrlsOptions> serviceUrlsOptions,
			IMediatorHandler mediator)
		{
			var serviceUrls = serviceUrlsOptions.Value;
			_mediator = mediator;
			_client = RestService.For<IFraudApi>(serviceUrls.Fraud);
		}

		/// <inheritdoc />
		public Task<PagedList<FraudSearchModel>> SearchAsync(FraudSearchCriteriaModel model) => _client.SearchAsync(model);

		/// <inheritdoc />
		public async Task SetStatus(FraudStatus status, List<Guid> items)
		{
			await _mediator.SendCommand(new SetFraudStatusCommand(status, items));

			await _client.SetStatusAsync(status, items);
		}

		/// <inheritdoc />
		public Task<FraudTicketModel> GetTicketAsync(Guid ticketId) => _client.GetTicketAsync(ticketId);

		/// <inheritdoc />
		public Task<FraudCount> GetFraudTicketCountAsync() => _client.GetFraudTicketCountAsync();

		/// <inheritdoc />
		public Task<RebuildResult> BuildFraudIndex() => _client.BuildFraudIndexAsync();
	}

	/// <summary>
	/// IFraudApi interface.
	/// </summary>
	public interface IFraudApi
	{
		/// <summary>
		/// Search through Fraud data for given condition.
		/// </summary>
		[Get("/search")]
		[Headers("Content-Type:application/json")]
		Task<PagedList<FraudSearchModel>> SearchAsync([Query] FraudSearchCriteriaModel model);

		/// <summary>
		/// Set fraud status for a ticket.
		/// </summary>
		[Post("/status/{fraudStatus}")]
		[Headers("Content-Type:application/json")]
		Task SetStatusAsync([Query] FraudStatus fraudStatus, [Body] List<Guid> items);

		/// <summary>
		/// Get ticket details.
		/// </summary>
		[Get("/ticket/{ticketId}")]
		[Headers("Content-Type:application/json")]
		Task<FraudTicketModel> GetTicketAsync([Query] Guid ticketId);

		/// <summary>
		/// Get fraud ticket count.
		/// </summary>
		[Get("/fraud-count")]
		[Headers("Content-Type:application/json")]
		Task<FraudCount> GetFraudTicketCountAsync();

		/// <summary>
		/// Build Fraud index.
		/// </summary>
		[Post("/index-fraud")]
		[Headers("Content-Type:application/json")]
		Task<RebuildResult> BuildFraudIndexAsync();
	}
}