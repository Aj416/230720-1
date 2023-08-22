using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models
{
	public class TicketModel
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The question asked by the customer in this ticket.
		/// </summary>
		public string Question { get; set; }

		/// <summary>
		/// The complexity value submitted by advocate who solved it.
		/// </summary>
		public int? Complexity { get; set; }

		/// <summary>
		/// Level of the ticket
		/// </summary>
		public int Level { get; set; }

		/// <summary>
		/// The CSAT submitted by the customer after closing the ticket.
		/// </summary>
		public int? Csat { get; set; }

		/// <summary>
		/// The NPS submitted by the customer after closing the ticket.
		/// </summary>
		public int? Nps { get; set; }

		/// <summary>
		/// The price of this ticket.
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// The number of how many times the ticket was abandoned by an advocate. It helps filtering
		/// ticket that are urgent (abandoned at least once).
		/// </summary>
		public int AbandonedCount { get; set; }

		/// <summary>
		/// The creation date of the ticket.
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The last modification date of the ticket.
		/// </summary>
		public DateTime? ModifiedDate { get; set; }

		/// <summary>
		/// The status of the ticket.
		/// </summary>
		public TicketStatusEnum Status { get; set; }

		/// <summary>
		/// The value of the status as int.
		/// </summary>
		public int StatusId => (int)Status;

		/// <summary>
		/// The information of the customer who created this ticket.
		/// </summary>
		public UserModel Customer { get; set; }

		/// <summary>
		/// Is the ticket for practicing.
		/// </summary>
		public bool IsPractice { get; set; }

		/// <summary>
		/// Is the ticket tagged completely.
		/// </summary>
		public bool IsTagged { get; set; }

		/// <summary>
		/// The status of tagging for the ticket.
		/// </summary>
		public TicketTagStatus? TagStatus { get; set; }

		/// <summary>
		/// The brand that this ticket belongs to.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The source of the ticket.
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// The reference.
		/// </summary>
		public string ReferenceId { get; set; }

		/// <summary>
		/// If the ticket in Reserved status, this will be the date of expiry.
		/// </summary>
		public DateTime? ReservationExpiryDate { get; set; }

		/// <summary>
		/// The advocate that is currenlty assigned for the ticket.
		/// </summary>
		public AdvocateModel Advocate { get; set; }

		/// <summary>
		/// The advocates that were ever associated with the ticket.
		/// </summary>
		public IEnumerable<AdvocateModel> AllAdvocates { get; set; }

		/// <summary>
		/// Transport type used for the ticket
		/// </summary>
		public TicketTransportType TransportType { get; set; }

		/// <summary>
		/// Ticket metadata
		/// </summary>
		public IDictionary<string, string> Metadata { get; set; }

		/// <summary>
		/// Customers probing answers
		/// </summary>
		public IEnumerable<ProbingAnswerModel> ProbingAnswers { get; set; }

		/// <summary>
		/// Server timestamp, can be used to calculate time left until ticket reservation expires
		/// </summary>
		public DateTime ServerTimestamp { get; set; }

		/// <summary>
		/// Tags assigned to the ticket
		/// </summary>
		public IList<TagModel> Tags { get; set; }

		/// <summary>
		/// The tags that are disabled for the ticket
		/// </summary>
		public IList<Guid> DisabledTags { get; set; }

		/// <summary>
		/// Ticket's escalation reason
		/// </summary>
		public TicketEscalationReason? EscalationReason { get; set; }

		/// <summary>
		/// Is the customer online.
		/// </summary>
		public bool IsCustomerOnline { get; set; }

		/// <summary>
		/// Is diagnosis required from L2 SuperSolver
		/// </summary>
		public bool IsDiagnosisRequired { get; set; }

		/// <summary>
		/// Whether or not escalated ticket was diagnosed correctly by a regular solver
		/// </summary>
		public bool? CorrectlyDiagnosed { get; set; }

		/// <summary>
		/// Does ticket contain SPOS lead?
		/// </summary>
		public bool? SposLead { get; set; }

		/// <summary>
		/// Ticket SPOS details
		/// </summary>
		public string SposDetails { get; set; }

		/// <summary>
		/// Fraud status on the ticket
		/// </summary>
		public FraudStatus FraudStatus { get; set; }

		/// <summary>
		/// Solver Average response time on the ticket
		/// </summary>
		public int? AverageSolverResponseTime { get; set; }

		/// <summary>
		/// Category associated with specific ticket
		/// </summary>
		public CategoryModel Category { get; set; }

		/// <summary>
		/// Solver longest response time on the ticket
		/// </summary>
		public int? MaxSolverResponseTime { get; set; }

		/// <summary>
		/// Solver all messages count
		/// </summary>
		public int SolverMessageCount { get; private set; }

		/// <summary>
		/// Customer all messages count
		/// </summary>
		public int CustomerMessageCount { get; private set; }

		/// <summary>
		/// Determines if SPOS email sent.
		/// </summary>
		public bool? SposEmailSent { get; set; }

		/// <summary>
		/// Determines if valid transfer is required.
		/// </summary>
		public bool IsValidTransferRequired { get; set; }

		/// <summary>
		/// Whether or not escalated ticket was valid transfer by a regular solver
		/// </summary>
		public bool? ValidTransfer { get; set; }

		/// <summary>
		/// Customer feedback relevant to support.
		/// </summary>
		public string AdditionalFeedBack { get; set; }

		/// <summary>
		/// Flag to indicate ticket is ready for processing
		/// </summary>
		public bool Ready { get; set; }
	}
}