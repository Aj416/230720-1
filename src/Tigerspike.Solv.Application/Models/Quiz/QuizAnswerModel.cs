using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Quiz question answer
	/// </summary>
	public class QuizAnswerModel
	{
		/// <summary>
		/// The answered question id
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The selected options
		/// </summary>
		public IEnumerable<Guid> Answers { get; set; }
	}
}