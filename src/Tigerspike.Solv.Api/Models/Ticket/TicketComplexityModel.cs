using System.ComponentModel.DataAnnotations;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Api.Models
{
	/// <summary>
	/// Ticket Complexity Model
	/// </summary>
	public class TicketComplexityModel
	{
		/// <summary>
		/// Complexity
		/// </summary>
		[Range(Ticket.MIN_COMPLEXITY, Ticket.MAX_COMPLEXITY)]
		public int Complexity { get; set; }
	}
}