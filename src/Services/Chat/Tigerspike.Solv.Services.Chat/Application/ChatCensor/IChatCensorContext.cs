using System.Collections.Generic;
using Tigerspike.Solv.Chat.Domain;

namespace Tigerspike.Solv.Chat.Application.ChatCensor
{
	public interface IChatCensorContext
	{
		/// <summary>
		/// Traverse through all censor strategies and ask them to censor the specified chat.
		/// </summary>
		/// <param name="message">The message to be censored</param>
		/// <param name="whitelist">Whitelist of phrases not to be covered by censor</param>
		/// <returns>True if any censor was done</returns>
		bool Censor(Message message, IList<string> whitelist);
	}
}