using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchTicketInfoResult : IResult
	{
		/// <summary>
		/// List of ticket info.
		/// </summary>
		IEnumerable<ITicketInfoResult> TicketInfo { get; set; }
	}

	public interface ITicketInfoResult
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		Guid Id { get; set; }

		/// <summary>
		/// The brand id of that ticket is for
		/// </summary>
		IBrandInfoResult Brand { get; set; }

		/// <summary>
		/// The price of this ticket.
		/// </summary>
		decimal Price { get; set; }

		/// <summary>
		/// The price of this ticket.
		/// </summary>
		decimal Fee { get; set; }

		/// <summary>
		/// The price of this ticket.
		/// </summary>
		decimal Total { get; set; }

		/// <summary>
		/// The creation date of the ticket.
		/// </summary>
		DateTime CreatedDate { get; set; }
	}

	public interface IBrandInfoResult
	{
		/// <summary>
		/// Brand Printable identifier.
		/// </summary>
		Guid Id { get; set; }

		/// <summary>
		/// Brand name.
		/// </summary>
		string Name { get; set; }
	}
}
