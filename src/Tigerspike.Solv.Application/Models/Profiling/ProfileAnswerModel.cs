using System;
using System.Collections.Generic;
using Tigerspike.Solv.Application.Models.Profile;

namespace Tigerspike.Solv.Application.Models
{
	public class ProfileAnswerModel
	{
		/// <summary>
		/// </summary>
		public Guid AdvocateApplicationId { get; set; } = Guid.NewGuid();

		/// <summary>
		/// </summary>
		public IList<ApplicationAnswerModel> ApplicationAnswers { get; set; } = new List<ApplicationAnswerModel>();
	}
}