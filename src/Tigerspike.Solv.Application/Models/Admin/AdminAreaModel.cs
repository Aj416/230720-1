using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Admin
{
	public class AdminAreaModel
	{
		public string Name { get; set; }

		public List<AdminQuestionModel> Questions { get; set; } = new List<AdminQuestionModel>();
	}
}