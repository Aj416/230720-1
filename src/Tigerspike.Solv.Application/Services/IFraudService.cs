using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Models.Fraud;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Api.Services
{
	/// <summary>
	///	Fraud service.
	/// </summary>
	public interface IFraudService
	{
		/// <summary>
		/// Searches the fraud service data.
		/// </summary>
		/// <param name="model">The fraud search model.</param>
		/// <returns>The paged list of fraud search model.</returns>
		Task<PagedList<FraudSearchModel>> SearchAsync(FraudSearchCriteriaModel model);

		/// <summary>
		/// Sets the fraud status of the items in the list.
		/// </summary>
		/// <param name="status">The fraud status.</param>
		/// <param name="items">The fraud items</param>
		Task SetStatus(FraudStatus status, List<Guid> items);

		/// <summary>
		/// Get the fraud ticket detail for ticket id.
		/// </summary>
		/// <param name="ticketId">The ticket id.</param>
		/// <returns>The fraud ticket detail for ticket id.</returns>
		Task<FraudTicketModel> GetTicketAsync(Guid ticketId);

		/// <summary>
		/// Get the fraud ticket count.
		/// </summary>
		/// <returns>The fraud ticket count.</returns>
		Task<FraudCount> GetFraudTicketCountAsync();

		/// <summary>
		/// Builds the fraud index. Used in non-production environments.
		/// </summary>
		/// <returns>The response message.</returns>
		Task<RebuildResult> BuildFraudIndex();
	}
}