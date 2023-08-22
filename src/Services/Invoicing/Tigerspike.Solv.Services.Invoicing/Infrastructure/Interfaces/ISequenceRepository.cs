using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Services.Invoicing.Domain;

namespace Tigerspike.Solv.Services.Invoicing.Infrastructure.Interfaces
{
	public interface ISequenceRepository : IRepository<Sequence>
	{
		/// <summary>
		/// Gets the next sequence number
		/// </summary>
		Task<int> Next(string sequenceName);
	}
}
