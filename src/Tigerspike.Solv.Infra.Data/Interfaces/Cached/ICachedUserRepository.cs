using System;
using System.Threading.Tasks;

namespace Tigerspike.Solv.Infra.Data.Interfaces.Cached
{
	public interface ICachedUserRepository
	{
		/// <summary>
		/// Get the cached info about user enabled status
		/// </summary>
		Task<bool?> IsUserEnabled(Guid userId);
	}
}