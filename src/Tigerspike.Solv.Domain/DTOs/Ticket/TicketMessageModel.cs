using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.DTOs
{
	/// <summary>
	/// The thread identifier
	/// </summary>
	public class TicketMessageModel
	{
		/// <summary>
		/// The author id
		/// </summary>
		public Guid? AuthorId { get; set; }

		/// <summary>
		/// The sender user type
		/// </summary>
		public UserType SenderType { get; set; }

		/// <summary>
		/// The revelant receivers
		/// </summary>
		public IEnumerable<UserType> RelevantTo { get; set; }

		/// <summary>
		/// The message content
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The message type
		/// </summary>
		public MessageType MessageType { get;set; }
	}
}