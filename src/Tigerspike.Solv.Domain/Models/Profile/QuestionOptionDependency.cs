using System;

namespace Tigerspike.Solv.Domain.Models.Profile
{
	public class QuestionOptionDependency
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
		public Guid QuestionOptionDependencyTargetId { get; set; }
	}
}