namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchAdvocateIdsForInvoicingCommand
	{
		/// <summary>
		///	Fetch advocate ids on basis of isauthorised flag.
		/// </summary>
		bool IsAuthorized { get; set; }

		/// <summary>
		/// Fetch advocate ids on basis of ispractice flag.
		/// </summary>
		bool IsPractice { get; set; }
	}
}
