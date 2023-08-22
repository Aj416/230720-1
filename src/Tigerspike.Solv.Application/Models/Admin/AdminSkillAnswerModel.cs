using System;

namespace Tigerspike.Solv.Application.Models.Admin
{
	public class AdminSkillAnswerModel
	{
		public Guid AdvocateId { get; set; }
		public Guid QuestionOptionId { get; set; }
		public string Answers { get; set; }
	}
}