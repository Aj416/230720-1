using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tigerspike.Solv.Core.Redis
{
	public interface IRedisSet<T>
	{
		long Count { get; }
		void Add(T value);
		void Remove(T value);
		bool Contains(T value);
		IEnumerable<T> GetAll();
	}
}