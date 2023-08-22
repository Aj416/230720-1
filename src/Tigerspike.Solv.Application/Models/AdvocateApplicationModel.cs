using System;
using System.Collections.Generic;
using System.Globalization;
using Tigerspike.Solv.Application.Models.Profile;

namespace Tigerspike.Solv.Application.Models
{
	public class AdvocateApplicationModel
	{
		/// <summary>
		/// Unique GUID primary key
		/// </summary>
		public Guid Id { get; private set; }

		/// <summary>
		/// The country of the advocate.
		/// </summary>
		public string Country { get; set; }

		/// <summary>
		/// The state of the advocate.
		/// </summary>
		public string State { get; set; }

		/// <summary>
		/// The email address of the advocate.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// First name of the advocate.
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Last name of the advocate.
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// The phone number of the advocate.
		/// </summary>
		public string Phone { get; set; }

		/// <summary>
		/// The source where the applicant heard about Solv.
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Whether or not the advocate is old enough to use this platform.
		/// </summary>
		public string IsAdult { get; set; }

		/// <summary>
		/// Whether or not the advocate consents to marketing.
		/// </summary>
		public string MarketingCheckbox { get; set; }

		/// <summary>
		/// The date the application was made
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Whether or not the Profiling Questionnaire has been completed
		/// </summary>
		public bool Questionnaire { get; set; }

		/// <summary>
		/// Date an invitation was sent
		/// </summary>
		public DateTime? InvitationDate { get; set; }

		public bool InternalAgent { get; set; }

		/// <summary>
		/// Answers given during the Application process
		/// </summary>
		public IList<ApplicationAnswerModel> ApplicationAnswers { get; set; } = new List<ApplicationAnswerModel>();

		/// <summary>
		/// The Address of the advocate.
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// The City of the advocate.
		/// </summary>
		public string City { get; set; }

		/// <summary>
		/// The Zip Code of the advocate.
		/// </summary>
		public string ZipCode { get; set; }

		/// <summary>
		/// Whether or not the advocate consents to Data Policy.
		/// </summary>
		public string DataPolicyCheckbox { get; set; }

		/// <summary>
		/// Password of the Advocate
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Method to make properties null so Json serialisation doesn't display it and sanitize first and last name
		/// </summary>
		public void Sanitize()
		{
			IsAdult = null;
			MarketingCheckbox = null;
			ApplicationAnswers = null;

			FirstName = !string.IsNullOrEmpty(FirstName) ? new CultureInfo("en").TextInfo.ToTitleCase(FirstName.ToLower().Trim()) : "";
			LastName = !string.IsNullOrEmpty(LastName) ? new CultureInfo("en").TextInfo.ToTitleCase(LastName.ToLower().Trim()) : "";
		}
	}
}