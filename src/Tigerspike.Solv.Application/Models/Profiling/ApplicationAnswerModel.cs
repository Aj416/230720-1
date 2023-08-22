using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Profile
{
	public class ApplicationAnswerModel
	{
		/// <summary>
		/// </summary>
		public Guid QuestionId { get; set; } = Guid.NewGuid();

		/// <summary>
		/// </summary>
		public IList<AnswerModel> Answers { get; set; } = new List<AnswerModel>();
	}
}