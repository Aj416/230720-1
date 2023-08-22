using Tigerspike.Solv.Messaging.Chat;

namespace Tigerspike.Solv.Services.Chat.Application.IntegrationEvents
{
	public class ChatActionOptionSideEffect : IChatActionOptionSideEffect
	{
        public ChatActionOptionSideEffect(int effect, string value)
        {
            Effect = effect;
            Value = value;
        }

		public int Effect { get; set; }
		public string Value { get; set; }
	}
}