using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Search
{
	public class TicketAbandonHistoryModel
	{
		public Guid TicketId { get; set; }
		public Guid? AdvocateId { get; set; }
		public DateTime CreatedDate { get; set; }
		public IEnumerable<string> Reasons { get; set; }
	}
}