using System;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Answer for the probing question
	/// </summary>
	public class ProbingResult
	{
		public Guid TicketId { get; set; }
		public Guid ProbingQuestionId { get; set; }
		public ProbingQuestion ProbingQuestion { get; set; }
		public Guid? ProbingQuestionOptionId { get; set; }
		public ProbingQuestionOption ProbingQuestionOption { get; set; }

		public ProbingResult() { }
		public ProbingResult(Guid ticketId, Guid probingQuestionId, Guid? probingQuestionOptionId)
		{
			TicketId = ticketId;
			ProbingQuestionId = probingQuestionId;
			ProbingQuestionOptionId = probingQuestionOptionId;
		}
	}
}
