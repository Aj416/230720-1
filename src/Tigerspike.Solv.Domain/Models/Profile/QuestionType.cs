using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Domain.Models.Profile
{
	public class QuestionType
	{
		/// <summary>
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// </summary>
		public bool IsMultiChoice { get; set; }

		/// <summary>
		/// </summary>
		public bool IsSlider { get; set; }

		/// <summary>
		/// </summary>
		public bool IsAllRequired { get; set; }

		/// <summary>
		/// </summary>
		public ICollection<Question> Questions { get; set; }
	}
}