using System;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Services
{
	public class CreateAbandonReasonCommand : Command
	{
		/// <summary>
		/// The brand Id
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Name of abandon reason
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Ticket flow action to take upon selecting this reason
		/// </summary>
		public TicketFlowAction? Action { get; set; }

		public CreateAbandonReasonCommand(Guid brandId, string name, TicketFlowAction? action = null)
		{
			BrandId = brandId;
			Name = name;
			Action = action;
		}

		public override bool IsValid() => BrandId != Guid.Empty;
	}
}