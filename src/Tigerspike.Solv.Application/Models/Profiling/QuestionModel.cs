using System;
using System.Collections.Generic;
using Tigerspike.Solv.Application.Models.Profiling;

namespace Tigerspike.Solv.Application.Models.Profile
{
	public class QuestionModel
	{
		/// <summary>
		/// </summary>
		public Guid QuestionId { get; set; } = Guid.NewGuid();

		/// <summary>
		/// </summary>
		public QuestionTypeModel QuestionType { get; set; } = new QuestionTypeModel();

		/// <summary>
		/// </summary>
		public int Order { get; set; }

		/// <summary>
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// </summary>
		public string SubTitle { get; set; }

		/// <summary>
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// </summary>
		public bool Optional { get; set; } = false;

		/// <summary>
		/// Gets or sets the header in question.
		/// </summary>
		public string Header { get; set; }

		/// <summary>
		/// Options to be displayed per row.
		/// </summary>
		public int? OptionsPerRow { get; set; }

		/// <summary>
		/// </summary>
		public IList<QuestionOptionModel> QuestionOptions { get; set; } = new List<QuestionOptionModel>();

		/// <summary>
		/// Gets or sets type of option combos if any.
		/// </summary>
		public ICollection<QuestionOptionComboModel> QuestionOptionCombos { get; set; }
	}
}