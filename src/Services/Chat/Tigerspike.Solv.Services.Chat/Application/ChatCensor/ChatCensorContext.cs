using System.Collections.Generic;
using System.Linq;
using Tigerspike.Solv.Chat.Application.ChatCensor.Strategies;
using Tigerspike.Solv.Chat.Domain;

namespace Tigerspike.Solv.Chat.Application.ChatCensor
{
	public class ChatCensorContext : IChatCensorContext
	{
		private readonly IEnumerable<IChatCensorStrategy> _chatCensorStrategies;

		public ChatCensorContext(IEnumerable<IChatCensorStrategy> chatCensorStrategies) =>
			_chatCensorStrategies = chatCensorStrategies;

		/// <inheritdoc />
		public bool Censor(Message message, IList<string> whitelist) =>
			_chatCensorStrategies.Select(s => s.Censor(message, whitelist)).Aggregate((a, b) => a || b);

	}
}