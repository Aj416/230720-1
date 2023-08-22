using Microsoft.AspNetCore.SignalR;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Chat.SignalR
{
	/// <summary>
	/// SignalR uses this internally to extract the user id from the token of the signed in user,
	/// since currently Auth0 adds 'auth0|' suffix to it.
	/// </summary>
	public class CustomUserIdProvider : IUserIdProvider
	{
		public virtual string GetUserId(HubConnectionContext connection) => connection.User.GetId().ToString();
	}
}
