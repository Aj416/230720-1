using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Profile
{
	public class AreaModel
	{
		/// <summary>
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// </summary>
		public IList<QuestionModel> Questions { get; set; } = new List<QuestionModel>();
	}
}