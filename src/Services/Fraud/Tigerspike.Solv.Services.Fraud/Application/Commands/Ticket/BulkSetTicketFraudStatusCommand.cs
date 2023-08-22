using System;
using System.Collections.Generic;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Services.Fraud.Enum;

namespace Tigerspike.Solv.Services.Fraud.Application.Commands.Ticket
{
	public class BulkSetTicketFraudStatusCommand : Command<Unit>
	{
		public FraudStatus FraudStatus { get; }

		public List<Guid> Items { get; }

		public BulkSetTicketFraudStatusCommand(FraudStatus fraudStatus, List<Guid> items)
		{
			FraudStatus = fraudStatus;
			Items = items;
		}

		public override bool IsValid()
		{
			return Items != null && Items.Count > 0;
		}
	}
}