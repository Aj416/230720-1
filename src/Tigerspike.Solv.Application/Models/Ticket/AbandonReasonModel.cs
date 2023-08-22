using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models
{
	public class AbandonReasonModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public TicketFlowAction? Action { get; set; }
		public string Description { get; set; }
	}
}