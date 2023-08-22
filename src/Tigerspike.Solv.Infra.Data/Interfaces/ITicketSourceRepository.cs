using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface ITicketSourceRepository : IRepository<TicketSource>
	{
		/// <summary>
		/// Gets ticket source by it's name
		/// </summary>
		/// <param name="source">The source name</param>
		Task<TicketSource> Get(string source);
	}
}