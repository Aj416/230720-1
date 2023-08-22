using System.Threading.Tasks;
using Tigerspike.Solv.Application.Models.Customer;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface ICustomerService
	{
		/// <summary>
		/// Returns Customer ticket list
		/// </summary>
		/// <param name="customerEmail">The customer Email</param>
		Task<CustomerModel> GetTickets(string customerEmail);
	}
}