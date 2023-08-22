using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class FetchAdvocateIdsForInvoicingCommand : IFetchAdvocateIdsForInvoicingCommand
	{
		/// <inheritdoc />
		public bool IsAuthorized { get; set; }

		/// <inheritdoc />
		public bool IsPractice { get; set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="isAuthorized">isAuthorized flag.</param>
		/// <param name="isPractice">isPractice flag.</param>
		public FetchAdvocateIdsForInvoicingCommand(bool isAuthorized, bool isPractice)
		{
			IsAuthorized = isAuthorized;
			IsPractice = isPractice;
		}
	}
}
