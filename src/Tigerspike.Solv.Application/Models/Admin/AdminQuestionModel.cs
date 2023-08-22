using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Admin
{
	public class AdminQuestionModel
	{
		/// <summary>
		/// </summary>
		public string Question { get; set; }

		/// <summary>
		/// </summary>
		public IList<string> Answers { get; set; } = new List<string>();
	}
}