using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tigerspike.Solv.Chat.Domain;

namespace Tigerspike.Solv.Chat.Application.ChatCensor.Strategies
{
	public abstract class BaseCensorStrategy : IChatCensorStrategy
	{
		public abstract string StrategyRegex { get; }

		public bool Censor(Message message, IList<string> whitelist)
		{
			var before = message.Content;

			message.Content = Regex.Replace(message.Content, StrategyRegex, match =>
			{
				var toReplace = match.ToString();
				return whitelist?.Contains(toReplace.ToLowerInvariant()) == true ?
					toReplace :
					new string('*', toReplace.Length);
			}, RegexOptions.Compiled | RegexOptions.IgnoreCase);

			return message.Content != before;
		}
	}
}