using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Domain.Models.Profile
{
	public class ApplicationAnswer
	{
		/// <summary>
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// </summary>
		public Guid AdvocateApplicationId { get; set; }

		/// <summary>
		/// </summary>
		public AdvocateApplication AdvocateApplication { get; set; }

		/// <summary>
		/// </summary>
		public Guid QuestionId { get; set; }

		/// <summary>
		/// </summary>
		public Question Question { get; set; }

		/// <summary>
		/// </summary>
		public ICollection<Answer> Answers { get; set; } = new List<Answer>();
	}
}