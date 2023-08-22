using System;

namespace Tigerspike.Solv.Domain.Models.Profile
{
	public class QuestionDependency
	{
		/// <summary>
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// </summary>
		public Guid QuestionOptionId { get; set; }

		/// <summary>
		/// </summary>
		public QuestionOption QuestionOption { get; set; }

		/// <summary>
		/// </summary>
		public Guid QuestionId { get; set; }

		/// <summary>
		/// </summary>
		public Question Question { get; set; }
	}
}