using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Profile
{
	public class QuestionOptionModel
	{
		/// <summary>
		/// </summary>
		public Guid QuestionOptionId { get; set; } = Guid.NewGuid();

		/// <summary>
		/// </summary>
		public int Order { get; set; }

		/// <summary>
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// </summary>
		public string SubText { get; set; }

		/// <summary>
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// </summary>
		public bool Optional { get; set; } = false;

		/// <summary>
		/// </summary>
		public IList<Guid> QuestionDependencies { get; set; } = new List<Guid>();

		/// <summary>
		/// </summary>
		public IList<Guid> OptionDependencies { get; set; } = new List<Guid>();
	}
}