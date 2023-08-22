using System;

namespace Tigerspike.Solv.Domain.Models.Profile
{
	public class Answer
	{
		/// <summary>
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// </summary>
		public Guid ApplicationAnswerId { get; set; }

		/// <summary>
		/// </summary>
		public ApplicationAnswer ApplicationAnswer { get; set; }

		/// <summary>
		/// </summary>
		public Guid? QuestionOptionId { get; set; }

		/// <summary>
		/// </summary>
		public QuestionOption QuestionOption { get; set; }

		/// <summary>
		/// </summary>
		public string StaticAnswer { get; set; }

		/// <summary>
		/// Gets or sets QuestionOptionComboId.
		/// </summary>
		public Guid? QuestionOptionComboId { get; set; }
	}
}