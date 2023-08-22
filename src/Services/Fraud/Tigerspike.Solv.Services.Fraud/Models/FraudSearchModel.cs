using System;
using System.Collections.Generic;
using System.Globalization;
using Tigerspike.Solv.Services.Fraud.Enum;

namespace Tigerspike.Solv.Services.Fraud.Models
{
	/// <summary>
	/// Reponse model for searched fraud tickets.
	/// </summary>
	public class FraudSearchModel
	{
		/// <summary>
		/// Format when date converted to string.
		/// </summary>
		public const string DateFormat = "dd/MM/yyyy";

		/// <summary>
		/// The date the application was made.
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The date the application was made in string format.
		/// Its for search functionality.
		/// </summary>
		public string CreatedDateText => CreatedDate.ToString(DateFormat, CultureInfo.InvariantCulture);

		/// <summary>
		/// The brand identifier.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The name of brand.
		/// </summary>
		public string BrandName { get; set; }

		public FraudStatus FraudStatus { get; set; }

		///<summary>
		///This is just a work-around for Elasticsearch to sort by name, currently the current settings although set perfectly, still doesn't work.
		///TODO: Just remove this property, when this question is answered https://discuss.elastic.co/t/case-insensitive-sort-doesnt-work/143192
		///</summary>
		public string BrandSortToken => BrandName.Replace(" ", "").ToLower();

		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The status of the ticket.
		/// </summary>
		public TicketStatus Status { get; set; }

		/// <summary>
		/// The status of the ticket as text
		/// </summary>
		public string StatusText => Status.ToString();

		///<summary>
		///This is just a work-around for Elasticsearch to sort by name, currently the current settings although set perfectly, still doesn't work.
		///TODO: Just remove this property, when this question is answered https://discuss.elastic.co/t/case-insensitive-sort-doesnt-work/143192
		///</summary>
		public string StatusSortToken => StatusText.Replace(" ", "").ToLower();

		/// <summary>
		/// Level of the ticket
		/// </summary>
		public TicketLevel Level { get; set; }

		/// <summary>
		/// The level of the ticket as text
		/// </summary>
		public string LevelText => Level.ToString();

		///<summary>
		///This is just a work-around for Elasticsearch to sort by name, currently the current settings although set perfectly, still doesn't work.
		///TODO: Just remove this property, when this question is answered https://discuss.elastic.co/t/case-insensitive-sort-doesnt-work/143192
		///</summary>
		public string LevelSortToken => LevelText.Replace(" ", "").ToLower();

		/// <summary>
		/// The advocates name.
		/// </summary>
		public string AdvocateName { get; set; }

		///<summary>
		///This is just a work-around for Elasticsearch to sort by name, currently the current settings although set perfectly, still doesn't work.
		///TODO: Just remove this property, when this question is answered https://discuss.elastic.co/t/case-insensitive-sort-doesnt-work/143192
		///</summary>
		public string AdvocateSortToken => AdvocateName?.Replace(" ", "").ToLower();

		// /// <summary>
		// /// The advocates name.
		// /// </summary>
		// public IEnumerable<string> AdvocatesHistory { get; set; }

		/// <summary>
		/// The list of observed risks for a particular ticket.
		/// </summary>
		public IEnumerable<string> FraudRisks { get; set; }

		public int? FraudLevel { get; set; }

		/// <summary>
		/// The cumulative point for all observed risk.
		/// </summary>
		public string FraudRiskLevel => FraudLevel.HasValue ? ((RiskLevel)FraudLevel.Value).ToString() : RiskLevel.None.ToString();

		///<summary>
		///This is just a work-around for Elasticsearch to sort by name, currently the current settings although set perfectly, still doesn't work.
		///TODO: Just remove this property, when this question is answered https://discuss.elastic.co/t/case-insensitive-sort-doesnt-work/143192
		///</summary>
		public string FraudRiskLevelSortToken => FraudRiskLevel.Replace(" ", "").ToLower();

		/// <summary>
		/// Metadata key value pair.
		/// </summary>
		public IDictionary<string, string> Metadata { get; set; }

		/// <summary>
		/// Customer specific details.
		/// </summary>
		public CustomerModel CustomerDetail { get; set; }

		/// <summary>
		/// Gets or sets question asked by customer.
		/// </summary>
		public string Question { get; set; }

		/// <summary>
		/// Gets or sets the IP address of the customer.
		/// </summary>
		public string IpAddress { get; set; }
	}
}