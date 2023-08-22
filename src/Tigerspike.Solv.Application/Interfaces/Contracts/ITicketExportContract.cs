using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Interfaces.Contracts
{
    public interface ITicketExportContract
    {
        /// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Creation timestamp
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Ticket transport type
		/// </summary>
		public TicketTransportType TransportType { get; set; }

		/// <summary>
		/// How many times ticket was rejected
		/// </summary>
		public int RejectionCount { get; set; }

		/// <summary>
		/// Timestamp of last acceptance of the ticket
		/// </summary>
		public DateTime? LastAcceptedDate { get; set; }

		/// <summary>
		/// Timestamp of last abandonment of the ticket
		/// </summary>
		public DateTime? LastAbandonedDate { get; set; }

		/// <summary>
		/// How many times ticket were abandoned
		/// </summary>
		public int AbandonedCount { get; set; }

		/// <summary>
		/// Timestamp of first message from Solver to Customer
		/// </summary>
		public DateTime? FirstMessageDate { get; set; }

		/// <summary>
		/// Time passed between creation of the ticket and first Solver's response
		/// </summary>
		public int? FirstResponseTime { get; set; }

		/// <summary>
		/// Timestamp of last action of marking the ticket as Solved, only calculated if ticket is Closed
		/// </summary>
		public DateTime? ResolvedDate { get; set; }

		/// <summary>
		/// How many times ticket were reopened
		/// </summary>
		public int ReopenedCount { get; set; }

		/// <summary>
		/// Timestamp of when ticket was closed
		/// </summary>
		public DateTime? ClosedDate { get; set; }

		/// <summary>
		/// What triggered closing of the ticket
		/// </summary>
		public ClosedBy? ClosedBy { get; set; }

		/// <summary>
		/// CSAT score for the ticket
		/// </summary>
		public int? Csat { get; set; }

		/// <summary>
		/// CSAT capture timestamp for the ticket
		/// </summary>
		public DateTime? CsatDate { get; set; }

		/// <summary>
		/// Metadata supplied on the ticket
		/// </summary>
		public string Metadata { get; set; }

		/// <summary>
		/// Name of the brand that ticket is associated with
		/// </summary>
		public string BrandName { get; set; }

		/// <summary>
		/// Time elapsed between creating a ticket and closing it
		/// </summary>
		public int? TotalHandlingTime { get; set; }

		/// <summary>
		/// Time elapsed between creating a ticket and marking it as solved (only calculated if ticket is closed)
		/// </summary>
		public int? TotalResolutionTime { get; set; }

		/// <summary>
		/// Current status of the ticket
		/// </summary>
		public TicketStatusEnum Status { get; set; }

		/// <summary>
		/// Reasons for last rejection of the ticket
		/// </summary>
		public string LastRejectionReasonsNames { get; set; }

		/// <summary>
		/// Complexity rated by the solver who solved the ticket
		/// </summary>
		public int? Complexity { get; set; }

		/// <summary>
		/// Reasons for last Abandon of the ticket
		/// </summary>
		public string LastAbandonmentReasonsNames { get; set; }

		/// <summary>
		/// Source of the ticket
		/// </summary>
		public string TicketSource { get; set; }
		
		/// <summary>
		/// L1 Tags of the ticket
		/// </summary>
		public string L1Tags { get; set; }

		/// <summary>
		/// L2 Tags of the ticket
		/// </summary>
		public string L2Tags { get; set; }

		/// <summary>
		/// Diagnosis of the ticket
		/// </summary>
		public string Diagnosis { get; set; }

		/// <summary>
		/// The NPS (Net Promoter Score) value of the ticket.
		/// </summary>
		public int? Nps { get; set; }

		/// <summary>
		/// Number of chaser emails have been sent to the Customer
		/// </summary>
		public int? ChaserEmails { get; set; }

		/// <summary>
		/// Waiting time for L1 solver
		/// </summary>
		public int? WaitingTimeL1 { get; set; }

		/// <summary>
		/// Number of chaser emails have been sent to the Customer
		/// </summary>
		public int? WaitingTimeL2 { get; set; }

		/// <summary>
		/// Time passed between Escalated of the ticket by Solver and accepted by Super Solver
		/// </summary>
		public int? SuperSolverFirstResponse { get; set; }

		/// <summary>
		/// Customer issue submitted via form.
		/// </summary>
		public string CustomerQuery { get; set; }

		/// <summary>
		/// Show time from when a ticket was closed until the customer submitted the CSAT
		/// </summary>
		public string CSATFirstResponse { get; }

		/// <summary>
		/// Add a new column 'CurrentTicketLevel'
		/// Show L1 if the current solver is L1
		/// Show L2 if currently assigned to L2
		/// </summary>
		public string CurrentTicketLevel { get; set; }

		/// <summary>
		/// Price of ticket.
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// Add a new column "NPSDate"
		/// Show date and time when the Customer submitted the NPS survey
		/// Format: DD/MM/YYYY HH:MM:SS
		/// if NPS is not submitted show blank
		/// </summary>
		public DateTime? NpsDate { get; set; }

		/// <summary>
		/// Add a new column 'DateEscalatedToL2'
		/// Format: DD/MM/YYYY HH:MM:SS
		/// Time and date when the ticket was escalated to L2, this includes the regular escalation flow
		/// (timeout, 2 rejected, 2 abandoned), admin escalated and L1 escalating the ticket to L2.
		/// </summary>
		public DateTime? DateEscalatedToL2 { get; set; }

		/// <summary>
		/// Add a new column 'LevelClosed'
		/// Description: Was the ticket closed by L1 or L2?
		/// L1: if L1 would get paid for the ticket
		/// L2: ticket was closed by L2, and L1 does not get paid
		/// </summary>
		public string LevelClosed { get; set; }

		/// <summary>
		/// Did the Customer return to their existing ticket? 1 = Yes, 0 = No
		/// </summary>
		public int RepeatExisting { get; set; }

		/// <summary>
		/// RiskCriteria
		/// </summary>
		public string RiskCriteria { get; set; }

		/// <summary>
		/// RiskLevel
		/// </summary>
		public string RiskLevel { get; set; }

		/// <summary>
		/// FraudConfirmed
		/// </summary>
		public string FraudConfirmed { get; set; }

		/// <summary>
		/// Did the Customer Eligible For Payment ticket? 1 = Yes, 0 = Blank
		/// </summary>
		public string SalesLead { get; set; }

		/// <summary>
		/// Ticket Escalated by 
		/// </summary>
		public string EscalatedBy { get; set; }

		/// <summary>
		/// L1 Sub Tags of the ticket
		/// </summary>
		public string L1SubTags { get; set; }

		/// <summary>
		/// L2 Sub Tags of the ticket
		/// </summary>
		public string L2SubTags { get; set; }

		/// <summary>
		/// Probing questions and answers per ticket
		/// </summary>
		public string ProbingQuestions { get; set; }

		/// <summary>
		/// Solver Response of the ticket
		/// </summary>
		public int SolverResponse { get; set; }

		/// <summary>
		/// Customer Response of the ticket
		/// </summary>
		public int CustomerResponse { get; set; }

		/// <summary>
		/// Avg Solver Response Time of the ticket
		/// </summary>
		public int AvgResponseTime { get; set; }

		/// <summary>
		/// Rating Comment of the ticket
		/// </summary>
		public string RatingComment { get; set; }

		/// <summary>
		/// Issue Type of the ticket
		/// </summary>
		public string IssueType { get; set; }

		/// <summary>
		/// Longest Time of the ticket
		/// </summary>
		public int? LongestTime { get; set; }

		/// <summary>
		/// Whether or not escalated ticket was valid transfer by a regular solver.
		/// </summary>
		public string InScopeForCrowd { get; set; }
		
		/// <summary>
		/// Repeted customer count L1
		/// </summary>
		public string RepeatedInL1 { get; set; }

		/// <summary>
		/// Repeted customer count L2
		/// </summary>
		public string RepeatedInL2 { get; set; }
    }
}