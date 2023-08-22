namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IRiskTransactionContextResult : IResult
	{
		string TrackingId { get; set; }
	}
}
