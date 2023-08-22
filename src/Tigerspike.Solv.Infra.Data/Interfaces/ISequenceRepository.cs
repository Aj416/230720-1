using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
    public interface ISequenceRepository: IRepository<Sequence>
    {
		/// <summary>
		/// Gets the next sequence number
		/// </summary>
		Task<int> Next(string sequenceName);
    }
}