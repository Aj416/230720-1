using System;
using System.Threading.Tasks;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface ITicketUrlService
	{
		Task<string> GetChatUrl(Guid ticketId, bool resume);
		Task<string> GetCustomerToken(Guid ticketId);
		Task<string> GetRateUrl(Guid ticketId, string culture, bool isEndChat = false);
	}
}
