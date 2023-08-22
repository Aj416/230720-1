namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchPaymentReceiverAccountIdResult : IResult
	{
		/// <summary>
		/// platformPaymentAccount Identifier.
		/// </summary>
		string PlatformPaymentAccountId { get; set; }
	}
}
