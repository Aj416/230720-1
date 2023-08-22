using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Domain.Models.Profile
{
	public class Area
	{
		/// <summary>
		/// Type of areas which is Your skills
		/// </summary>
		public const string Skills = "Your skills";

		/// <summary>
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// </summary>
		public int Order { get; set; }

		/// <summary>
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Determines whether the profile area is enabled.
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// </summary>
		public ICollection<Question> Questions { get; set; }
	}
}