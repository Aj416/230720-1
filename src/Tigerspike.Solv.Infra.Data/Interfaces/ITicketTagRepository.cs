using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface ITicketTagRepository : IRepository<TicketTag>
	{
		/// <summary>
		/// Get pending tickets that were tagged (only for specific tags) by solver.
		/// </summary>
		/// <param name="tagIds">The array of tag ids.</param>
		/// <param name="advocateId">The advocate identifier.</param>
		/// <returns>Returns the concerned ticket ids.</returns>
		Task<IEnumerable<Guid>> GetEscalatedTicketIds(HashSet<Guid> tagIds, Guid advocateId);
	}
}
