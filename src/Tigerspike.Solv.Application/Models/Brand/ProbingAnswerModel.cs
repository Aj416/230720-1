using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models
{
	public class ProbingAnswerModel
	{
		public string QuestionText { get; set; }
		public string QuestionCode { get; set; }
		public string Answer { get; set; }
		public TicketFlowAction? Action { get; set; }

		public ProbingAnswerModel(string questionText, string questionCode, string answer, TicketFlowAction? action)
		{
			QuestionText = questionText;
			QuestionCode = questionCode;
			Answer = answer;
			Action = action;
		}

	}
}