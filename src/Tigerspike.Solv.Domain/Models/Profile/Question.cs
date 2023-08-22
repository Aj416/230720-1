using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Domain.Models.Profile
{
	public class Question
	{
		/// <summary>
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// </summary>
		public Guid AreaId { get; set; }

		/// <summary>
		/// </summary>
		public Area Area { get; set; }

		/// <summary>
		/// </summary>
		public Guid QuestionTypeId { get; set; }

		/// <summary>
		/// </summary>
		public QuestionType QuestionType { get; set; }

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
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the header in question.
		/// </summary>
		public string Header { get; set; }

		/// <summary>
		/// </summary>
		public bool Optional { get; set; }

		/// <summary>
		/// Options to be displayed per row.
		/// </summary>
		public int? OptionsPerRow { get; set; }

		/// <summary>
		/// </summary>
		public ICollection<QuestionOption> QuestionOptions { get; set; }

		/// <summary>
		/// </summary>
		public ICollection<ApplicationAnswer> ApplicationAnswers { get; set; }

		/// <summary>
		/// </summary>
		public ICollection<QuestionDependency> QuestionDependencies { get; set; }

		/// <summary>
		/// Gets or sets collection of optioncombos, present usually when the question type will be a new type - MulipleOptionSet
		/// </summary>
		public ICollection<QuestionOptionCombo> QuestionOptionCombos { get; set; }
	}
}