using System;

namespace Tigerspike.Solv.Application.Models.Profile
{
	public class AnswerModel
	{
		/// <summary>
		/// </summary>
		public Guid? QuestionOptionId { get; set; }

		/// <summary>
		/// Gets or sets questionoptioncombo id.
		/// </summary>
		public Guid? QuestionOptionComboId { get; set; }

		/// <summary>
		/// </summary>
		public string StaticAnswer { get; set; }
	}
}