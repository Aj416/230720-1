using System;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Application.Models.Fraud
{
	public class SerialNumber : ISerialNumber
	{
		public DateTime CreatedDate { get; set; }
		public Guid CustomerId { get; set; }
		public Guid? SolverId { get; set; }
		public Guid TicketId { get; set; }
		public string Serial { get; set; }
	}
}