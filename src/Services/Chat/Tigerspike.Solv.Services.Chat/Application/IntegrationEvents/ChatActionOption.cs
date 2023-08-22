using System.Collections.Generic;
using Tigerspike.Solv.Messaging.Chat;

namespace Tigerspike.Solv.Services.Chat.Application.IntegrationEvents
{
	public class ChatActionOption : IChatActionOption
	{
		public ChatActionOption(string label, string value, bool isSelected, bool isSuggested)
		{
			Label = label;
			Value = value;
			IsSelected = isSelected;
			IsSuggested = isSuggested;
		}

		public string Label { get; set; }
		public string Value { get; set; }
		public bool IsSelected { get; set; }
		public bool IsSuggested { get; set; }
		public List<IChatActionOptionSideEffect> SideEffects { get; set; }
	}
}