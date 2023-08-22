using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface ITicketEscalationConfigRepository : IRepository<TicketEscalationConfig>
	{

		/// <summary>
		/// Gets escalation flow config for specified brand & source
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="ticketSourceId">The ticket source id</param>
		Task<TicketEscalationConfig> Get(Guid brandId, int? ticketSourceId);

		/// <summary>
		/// Returns list of brands that have escalation flow configured
		/// </summary>
		/// <returns></returns>
		Task<IList<Guid>> GetBrands();

	}
}