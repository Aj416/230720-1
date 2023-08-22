using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Search
{
	public class AdvocateApplicationSearchResponseModel
	{
		/// <summary>
		/// Unique GUID primary key
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The country of the advocate.
		/// </summary>
		public string Country { get; set; }

		/// <summary>
		/// The source where the applicant heard about Solv.
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// The list of languages candidate is fluent in
		/// </summary>
		public IEnumerable<string> Language { get; set; }
		/// <summary>
		/// The list of skills of candidate (and level of expertise)
		/// </summary>
		public IEnumerable<string> Skills { get; set; }

		/// <summary>
		/// The date the application was made
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Whether or not the Profiling Questionnaire has been completed
		/// </summary>
		public bool Questionnaire { get; set; }

		/// <summary>
		/// Date an invitation was sent.
		/// </summary>
		public DateTime? InvitationDate { get; set; }

		/// <summary>
		/// Full name of candidate.
		/// </summary>
		public string FullName { get; set; }

		/// <summary>
		/// Email of the advocate.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// advocate phone number
		/// </summary>
		public string Phone { get; set; }
	}
}