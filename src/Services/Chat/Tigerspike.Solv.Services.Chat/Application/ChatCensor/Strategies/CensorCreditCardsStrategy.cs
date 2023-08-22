namespace Tigerspike.Solv.Chat.Application.ChatCensor.Strategies
{
	public class CensorCreditCardsStrategy : BaseCensorStrategy
	{
		public override string StrategyRegex => @"(?<!\S)\b((\w?\d{4}[- ]?){3}([- ]?\d{1,7}))\b";
	}
}