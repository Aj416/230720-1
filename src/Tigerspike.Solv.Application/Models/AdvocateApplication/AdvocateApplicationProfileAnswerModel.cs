using System;

namespace Tigerspike.Solv.Application.Models
{
	public class AdvocateApplicationProfileAnswerModel
	{
		/// <summary>
		/// QuestionOption Identifier.
		/// </summary>
		public Guid? QuestionOptionId { get; set; }

		/// <summary>
		/// Static Answer incase its other than an option.
		/// </summary>
		public string StaticAnswer { get; set; }

		/// <summary>
		/// Gets or sets QuestionOptionComboId.
		/// </summary>
		public Guid? QuestionOptionComboId { get; set; }
	}
}
