using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models
{
	public class AdvocateApplicationProfileModel
	{
		/// <summary>
		/// Question identifier.
		/// </summary>
		public Guid QuestionId { get; set; }

		/// <summary>
		/// Answers selected by advocate for the question.
		/// </summary>
		public IList<AdvocateApplicationProfileAnswerModel> Answers { get; set; } = new List<AdvocateApplicationProfileAnswerModel>();
	}
}
