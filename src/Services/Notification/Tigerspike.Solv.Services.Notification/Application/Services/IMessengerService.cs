using System.Threading.Tasks;

namespace Tigerspike.Solv.Services.Notification.Application.Services
{
	public interface IMessengerService
	{
		/// <summary>
		/// Posts a message to the messenger.
		/// </summary>
		/// <param name="conversationId">The conversation id.</param>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		Task PostMessage(string conversationId, string text);
	}
}