using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Enum;
using Tigerspike.Solv.Services.Fraud.Models;

namespace Tigerspike.Solv.Services.Fraud.Application.Services
{
	/// <summary>
	/// IFraudService interface.
	/// </summary>
	public interface IFraudService
	{
		/// <summary>
		/// Returns rules for a particular status.
		/// </summary>
		/// <param name="status">The ticket status.</param>
		/// <returns>Rules for a particular status.</returns>
		List<RuleModel> GetRules(int status);

		/// <summary>
		/// Returns rules applied to a ticket.
		/// </summary>
		/// <param name="ticketId">The ticket id.</param>
		/// <returns>Rules applied to a ticket.</returns>
		List<RuleModel> GetRulesAppliedToTicket(Guid ticketId);

		/// <summary>
		/// Returns ticket detail for particular ticket id.
		/// </summary>
		/// <param name="ticketId">The ticket id.</param>
		/// <returns>Ticket detail for particular id.</returns>
		TicketModel GetTicketDetails(Guid ticketId);
	}
}