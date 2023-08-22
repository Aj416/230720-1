namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Ticket transition request model
	/// </summary>
	public class TicketDiagnosisModel
	{
		/// <summary>
		/// Whether escalated ticket was correctly diagnosed or not
		/// </summary>
		public bool? CorrectlyDiagnosed { get; set; }
	}
}