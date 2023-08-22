using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Admin
{
	public class AdminAnswerModel
	{
		public Guid AdvocateId { get; set; }

		public Guid AreaId { get; set; }

		public Guid QuestionId { get; set; }
		public IList<string> Answers { get; set; } = new List<string>();
	}
}
