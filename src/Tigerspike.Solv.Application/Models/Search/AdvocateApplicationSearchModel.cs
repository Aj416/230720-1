using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Tigerspike.Solv.Application.Models.Search
{
	public class AdvocateApplicationSearchModel
	{
		/// <summary>
		/// Format when date converted to string.
		/// </summary>
		public const string DateFormat = "dd/MM/yyyy";

		/// <summary>
		/// Unique GUID primary key
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The country of the advocate.
		/// </summary>
		public string Country { get; set; }

		/// <summary>
		/// First name of the advocate.
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Last name of the advocate.
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// Email of the advocate.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// advocate phone number
		/// </summary>
		public string Phone { get; set; }

		/// <summary>
		/// The source where the applicant heard about Solv.
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// The list of languages candidate is fluent in
		/// </summary>
		public IEnumerable<string> Language { get; set; }

		public string LanguageSortToken => string.Join(",", Language.Select(x => x.ToLower()));

		/// <summary>
		/// The list of languages candidate is fluent in
		/// </summary>
		public IEnumerable<ProfileSkillModel> Skills { get; set; }

		public string SkillsSortToken => string.Join(",", Skills.Select(x => x.Display));

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

		/// <summary>
		/// The date the application was made in string format.
		/// Its for search functionality.
		/// </summary>
		public string CreatedDateText => CreatedDate.ToString(DateFormat, CultureInfo.InvariantCulture);

		/// <summary>
		/// The date the application was made in string format.
		/// Its for search functionality.
		/// </summary>
		public string InvitationDateText => InvitationDate.HasValue ? InvitationDate.Value.ToString(DateFormat, CultureInfo.InvariantCulture): string.Empty;

		/// <summary>
		/// Full name of candidate.
		/// </summary>
		public string FullName => $"{FirstName} {LastName}";

		///<summary>
		///This is just a work-around for Elasticsearch to sort by name, currently the current settings although set perfectly, still doesn't work.
		///TODO: Just remove this property, when this question is answered https://discuss.elastic.co/t/case-insensitive-sort-doesnt-work/143192
		///</summary>
		public string NameSortToken => FullName.Replace(" ", "").ToLower();

		///<summary>
		///This is just a work-around for Elasticsearch to sort by name, currently the current settings although set perfectly, still doesn't work.
		///TODO: Just remove this property, when this question is answered https://discuss.elastic.co/t/case-insensitive-sort-doesnt-work/143192
		///</summary>
		public string CountrySortToken => $"{Country}".Replace(" ", "").ToLower();

		///<summary>
		///This is just a work-around for Elasticsearch to sort by name, currently the current settings although set perfectly, still doesn't work.
		///TODO: Just remove this property, when this question is answered https://discuss.elastic.co/t/case-insensitive-sort-doesnt-work/143192
		///</summary>
		public string SourceSortToken => $"{Source}".Replace(" ", "").ToLower();

		/// <summary>
		/// Application Status.
		/// </summary>
		public int ApplicationStatus { get; set; }

	}
}