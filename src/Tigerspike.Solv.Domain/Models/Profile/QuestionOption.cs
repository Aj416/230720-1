using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Models.Profile
{
	public class QuestionOption
	{
		/// <summary>
		/// Question Option : Complex technical support
		/// </summary>
		public const string ComplexTechnicalSupport = "Complex technical support";

		/// <summary>
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// </summary>
		public Guid QuestionId { get; set; }

		/// <summary>
		/// </summary>
		public Question Question { get; set; }

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
		/// Business meaning of the question
		/// </summary>
		public QuestionBusinessValue? BusinessValue { get; set; }

		/// <summary>
		/// Gets or sets questionoptioncombo id.
		/// </summary>
		public Guid? QuestionOptionComboId { get; set; }

		/// <summary>
		/// </summary>
		public ICollection<Answer> Answers { get; set; }

		/// <summary>
		/// </summary>
		public ICollection<QuestionDependency> QuestionDependencies { get; set; }

		/// <summary>
		/// </summary>
		public ICollection<QuestionOptionDependency> QuestionOptionDependencies { get; set; }
	}
}