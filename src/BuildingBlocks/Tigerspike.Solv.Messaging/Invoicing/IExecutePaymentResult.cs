namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IExecutePaymentResult : IResult
	{
		string ReferenceId { get; set; }
	}
}
