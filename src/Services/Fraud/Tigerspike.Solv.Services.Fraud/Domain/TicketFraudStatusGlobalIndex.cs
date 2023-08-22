using ServiceStack.Aws.DynamoDb;
using ServiceStack.DataAnnotations;

namespace Tigerspike.Solv.Services.Fraud.Domain
{
	public class TicketFraudStatusGlobalIndex : IGlobalIndex<Ticket>
	{
		/// <summary>
		/// Gets or sets the fraud status of ticket.
		/// </summary>
		[HashKey]
		public string FraudStatus { get; set; }

		/// <summary>
		/// Gets or sets the ticket Id.
		/// </summary>
		[Index]
		public string TicketId { get; set; }
	}
}
