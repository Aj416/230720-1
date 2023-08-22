namespace Tigerspike.Solv.Chat.Application.ChatCensor.Strategies
{
	public class CensorEmailStrategy : BaseCensorStrategy
	{
		public override string StrategyRegex => @"\b([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})\b";
	}
}