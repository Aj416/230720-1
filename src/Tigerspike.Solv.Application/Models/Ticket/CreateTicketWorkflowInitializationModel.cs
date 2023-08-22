using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Ticket
{
	/// <summary>
	/// The ticket creation workflow initization DTO
	/// </summary>
	public class CreateTicketWorkflowInitializationModel
	{
		/// <summary>
		/// The ticket identifier
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// The brand identifier
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The ticket level
		/// </summary>
		public TicketLevel Level { get; set; }

		/// <summary>
		/// The flag to indicate if ticket is a practice ticket or not
		/// </summary>
		public bool IsPractice { get; set; }
	}
}
