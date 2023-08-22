using System;
using System.Collections.Generic;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.DTOs;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Commands.Chat
{
	/// <summary>
	/// The command to send initial chat messages
	/// </summary>
	public class SendInitialChatMessageCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// Created date
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The customer identifier
		/// </summary>
		public Guid CustomerId { get; set; }

		/// <summary>
		/// The brand identifier
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The ticket transport type
		/// </summary>
		public TicketTransportType TransportType { get; set; }

		/// <summary>
		/// Question
		/// </summary>
		public string Question { get; set; }

		/// <summary>
		/// The flag which indicates if the ticket is a practice ticket or not
		/// </summary>
		public bool IsPractice { get; set; }

		/// <summary>
		/// The thread identifier
		/// </summary>
		public string ThreadId { get; set; }

		/// <summary>
		/// The messages to be sent
		/// </summary>
		public List<TicketMessageModel> Messages { get; set; }

		/// <summary>
		/// The method to validate the instance
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() => TicketId != Guid.Empty;
	}
}
