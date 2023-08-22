namespace Tigerspike.Solv.Chat.Application.ChatCensor.Strategies
{
	public class CensorPhoneNumbersStrategy : BaseCensorStrategy
	{
		public override string StrategyRegex => @"(?<!\S)\(?(?:\+|0+)?\d+\)?[- .]?(?:\(\d+\))?[- .]?\d{2,}[. -]? ?\d{2,}[ -.]? ?\d{2,}\b";
	}
}