using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Domain.Models.Profile
{
	public class QuestionOptionCombo
	{
		/// <summary>
		/// Gets or sets Id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets question identifier.
		/// </summary>
		public Guid QuestionId { get; set; }

		/// <summary>
		/// Gets or sets the title for set of options.
		/// </summary>
		public string ComboOptionTitle { get; set; }

		/// <summary>
		/// Gets or sets the type of option's.
		/// </summary>
		public QuestionComboOptionType ComboOptionType { get; set; }

		/// <summary>
		/// Gets or sets combos order.
		/// </summary>
		public int Order { get; set; }

		/// <summary>
		/// Determines whethere the option combo is enabled.
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// Determines the options per row.
		/// </summary>
		public int OptionsPerRow { get; set; }

		/// <summary>
		/// Gets or sets list of question options.
		/// </summary>
		public ICollection<QuestionOption> ComboOptions { get; set; }
	}

	public enum QuestionComboOptionType
	{
		SingleChoice,
		MultiChoice
	}
}
