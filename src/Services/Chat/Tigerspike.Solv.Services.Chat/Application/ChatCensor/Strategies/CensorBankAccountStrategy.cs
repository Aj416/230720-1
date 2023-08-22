namespace Tigerspike.Solv.Chat.Application.ChatCensor.Strategies
{
	public class CensorBankAccountStrategy : BaseCensorStrategy
	{
		public override string StrategyRegex => @"[a-zA-Z]{2}[0-9]{2}[- ]?[a-zA-Z0-9]{4}[0-9]{7}([a-zA-Z0-9]?){0,16}";
	}
}