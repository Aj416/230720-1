using System.ComponentModel.DataAnnotations;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Api.Models
{
	/// <summary>
	/// Ticket NPS Model
	/// </summary>
	public class TicketNpsModel
	{
		/// <summary>
		/// NPS value.
		/// </summary>
		[Range(Ticket.MIN_NPS, Ticket.MAX_NPS)]
		public int Nps { get; set; }
	}
}