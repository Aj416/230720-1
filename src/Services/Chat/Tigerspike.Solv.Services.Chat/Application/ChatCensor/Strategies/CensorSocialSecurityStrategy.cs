namespace Tigerspike.Solv.Chat.Application.ChatCensor.Strategies
{
	public class CensorSocialSecurityStrategy : BaseCensorStrategy
	{
		public override string StrategyRegex => @"(?<!\S)\b(?!000)(?!666)(?!9)[0-9]{3}[ -]?(?!00)[0-9]{2}[ -]?(?!0000)[0-9]{4}\b";
	}
}