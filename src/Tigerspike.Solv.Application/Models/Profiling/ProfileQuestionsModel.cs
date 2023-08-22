using System.Collections.Generic;
using Tigerspike.Solv.Application.Models.Profile;

namespace Tigerspike.Solv.Application.Models
{
	public class ProfileQuestionsModel
	{
		/// <summary>
		/// </summary>
		public IList<AreaModel> Areas { get; set; } = new List<AreaModel>();
	}
}