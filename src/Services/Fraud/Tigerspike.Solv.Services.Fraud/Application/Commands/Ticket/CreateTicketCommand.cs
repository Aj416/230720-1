using System;
using System.Collections.Generic;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Services.Fraud.Application.Commands.Ticket
{
	public class CreateTicketCommand : Command<Unit>
	{
		public Guid BrandId { get; set; }
		public Guid TicketId { get; set; }
		public string BrandName { get; set; }
		public int Level { get; set; }
		public int Status { get; set; }
		public Guid? CustomerId { get; set; }
		public string AdvocateName { get; set; }
		public IDictionary<string, string> Metadata { get; set; }
		public string CustomerFirstName { get; set; }
		public string CustomerLastName { get; set; }
		public string CustomerEmail { get; set; }
		public string Question { get; set; }
		public string IpAddress { get; set; }

		public CreateTicketCommand(Guid brandId, Guid ticketId, int level, int status, Guid? customerId,
			string advocateName, string brandName, IDictionary<string, string> metadata, string customerFirstName, string customerLastName, string customerEmail, string question, string ipAddress)
		{
			BrandId = brandId;
			TicketId = ticketId;
			Level = level;
			Status = status;
			CustomerId = customerId;
			AdvocateName = advocateName;
			BrandName = brandName;
			Metadata = metadata;
			CustomerFirstName = customerFirstName;
			CustomerLastName = customerLastName;
			CustomerEmail = customerEmail;
			Question = question;
			IpAddress = ipAddress;
		}

		public override bool IsValid() => BrandId != Guid.Empty && TicketId != Guid.Empty && CustomerId.HasValue;
	}
}