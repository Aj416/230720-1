using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Models.Client;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface IClientService
	{
		/// <summary>
		/// Returns client details
		/// </summary>
		/// <param name="clientId">The client/user id</param>
		Task<ClientModel> GetClient(Guid clientId);

		/// <summary>
		/// Create new client account
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="firstName">User first name</param>
		/// <param name="lastName">User last name</param>
		/// <param name="email">User password</param>
		/// <param name="phone">User phone (not required)</param>
		/// <param name="password">User password</param>
		Task<Guid?> CreateClient(Guid brandId, string firstName, string lastName, string email, string phone, string password);
	}
}
