using System.ComponentModel.DataAnnotations;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Api.Models
{
	/// <summary>
	/// Ticket Csat Model
	/// </summary>
	public class TicketCsatModel
	{
		/// <summary>
		/// Csat
		/// </summary>
		[Range(Ticket.MIN_CSAT, Ticket.MAX_CSAT)]
		public int Csat { get; set; }
	}
}