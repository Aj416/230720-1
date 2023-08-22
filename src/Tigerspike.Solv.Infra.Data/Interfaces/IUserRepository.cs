using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface IUserRepository : IRepository<User>
	{
		/// <summary>
		/// Gets the user based on the email
		/// </summary>
		/// <param name="email">The user's email</param>
		Task<User> GetByEmail(string email);

		/// <summary>
		/// Returns enabled status of the user
		/// </summary>
		/// <param name="userId">The user id</param>
		Task<bool?> IsUserEnabled(Guid userId);

	}
}