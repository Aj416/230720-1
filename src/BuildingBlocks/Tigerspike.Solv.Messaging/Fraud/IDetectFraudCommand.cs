using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Fraud
{
	/// <summary>
	/// Model with all fraud relevant data points.
	/// </summary>
	public interface IDetectFraudCommand
	{
		/// <summary>
		/// Gets or sets serial number info for last week.
		/// </summary>
		IList<ISerialNumber> SerialNumberInfoForLastWeek { get; set; }

		/// <summary>
		/// Gets or sets serial number info for last three days.
		/// </summary>
		IList<ISerialNumber> SerialNumberInfoForLastThreeDays { get; set; }

		/// <summary>
		/// Gets or sets customer info for last day.
		/// </summary>
		IList<ICustomer> CustomerInfoForLastDay { get; set; }

		/// <summary>
		/// Gets or sets brandid.
		/// </summary>
		Guid BrandId { get; set; }

		/// <summary>
		/// Gets or sets solver ip.
		/// </summary>
		string SolverIp { get; set; }

		/// <summary>
		/// Gets or sets brand name.
		/// </summary>
		string BrandName { get; set; }

		/// <summary>
		/// Gets or sets customer ip.
		/// </summary>
		string CustomerIp { get; set; }

		/// <summary>
		/// Gets or sets list of response time.
		/// </summary>
		IList<int> ResponseTimes { get; set; }

		/// <summary>
		/// Gets or sets current tickets response time.
		/// </summary>
		public int? CurrentResponseTimeInMinutes { get; set; }

		/// <summary>
		/// Gets or sets close time.
		/// </summary>
		int? CloseTime { get; set; }

		/// <summary>
		/// Gets or sets csat.
		/// </summary>
		int? Csat { get; set; }

		/// <summary>
		/// Gets or sets ticket status.
		/// </summary>
		int TicketStatus { get; set; }

		/// <summary>
		/// Gets or sets ticket level.
		/// </summary>
		int Level { get; set; }

		/// <summary>
		/// Gets or sets Ticket created date.
		/// </summary>
		DateTime CreatedDate { get; set; }

		/// <summary>
		/// Gets or sets Ticket id.
		/// </summary>
		Guid TicketId { get; set; }

		/// <summary>
		/// Gets or sets Advocate name.
		/// </summary>
		string AdvocateName { get; set; }

		/// <summary>
		/// Gets or sets Customer id.
		/// </summary>
		Guid? CustomerId { get; set; }

		/// <summary>
		/// Gets or sets Closed By Customer.
		/// </summary>
		bool? ClosedByCustomer { get; set; }

		/// <summary>
		/// Key value pair for metadata.
		/// </summary>
		IDictionary<string, string> Metadata { get; set; }

		/// <summary>
		/// Customer first name.
		/// </summary>
		string CustomerFirstName { get; set; }

		/// <summary>
		/// Customer last name.
		/// </summary>
		string CustomerLastName { get; set; }

		/// <summary>
		/// Customer email id.
		/// </summary>
		string CustomerEmail { get; set; }

		/// <summary>
		/// The question asked by the customer in this ticket.
		/// </summary>
		public string Question { get; set; }
	}
}