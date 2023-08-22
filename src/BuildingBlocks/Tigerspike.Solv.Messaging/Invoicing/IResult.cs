namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IResult
	{
		/// <summary>
		/// Determines if the request was a success.
		/// </summary>
		bool IsSuccess { get; set; }
	}
}
