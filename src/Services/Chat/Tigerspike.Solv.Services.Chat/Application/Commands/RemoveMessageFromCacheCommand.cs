using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Services.Chat.Application.Models;

namespace Tigerspike.Solv.Chat.Application.Commands
{
	public class RemoveMessageFromCacheCommand : Command
	{
		public string Key { get; private set; }

		public MessageModel Message { get; private set; }

		public RemoveMessageFromCacheCommand(string key, MessageModel message)
		{
			Key = key;
			Message = message;
		}

		public override bool IsValid()
		{
			return !string.IsNullOrWhiteSpace(Key);
		}
	}
}
