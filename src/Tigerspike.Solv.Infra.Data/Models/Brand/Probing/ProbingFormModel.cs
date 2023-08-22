using System.Collections.Generic;

namespace Tigerspike.Solv.Infra.Data.Models
{
	public class ProbingFormModel
	{
		public string Title { get; set; }
		public IEnumerable<ProbingQuestionModel> Questions { get; set; }
	}
}
