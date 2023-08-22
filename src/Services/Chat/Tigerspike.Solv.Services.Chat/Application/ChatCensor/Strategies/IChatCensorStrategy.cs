using System.Collections.Generic;
using Tigerspike.Solv.Chat.Domain;

namespace Tigerspike.Solv.Chat.Application.ChatCensor.Strategies
{
	public interface IChatCensorStrategy
	{
		bool Censor(Message message, IList<string> whitelist);
	}
}