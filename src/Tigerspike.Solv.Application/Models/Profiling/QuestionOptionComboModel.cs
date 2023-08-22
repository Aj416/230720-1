using System;
using System.Collections.Generic;
using Tigerspike.Solv.Application.Models.Profile;

namespace Tigerspike.Solv.Application.Models.Profiling
{
	public class QuestionOptionComboModel
	{
		/// <summary>
		/// Gets or sets Id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the title for set of options.
		/// </summary>
		public string ComboOptionTitle { get; set; }

		/// <summary>
		/// Gets or sets the type of option's.
		/// </summary>
		public string ComboOptionType { get; set; }


		/// <summary>
		/// Determines whethere the option combo is enabled.
		/// </summary>
		public bool Enabled { get; set; }


		/// <summary>
		///  Determines the options per row.
		/// </summary>
		public int OptionsPerRow { get; set; }


		/// <summary>
		/// Gets or sets list of question options.
		/// </summary>
		public ICollection<QuestionOptionModel> ComboOptions { get; set; }
	}
}
