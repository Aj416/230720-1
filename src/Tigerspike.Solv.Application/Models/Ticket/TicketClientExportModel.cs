using System;
using Tigerspike.Solv.Application.Interfaces.Contracts;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Ticket
{
    public class TicketClientExportModel : ITicketExportContract
    {
        /// <inheritdoc/>
		public Guid Id { get; set; }

		/// <inheritdoc/>
		public DateTime CreatedDate { get; set; }

		/// <inheritdoc/>
		public TicketTransportType TransportType { get; set; }

		/// <inheritdoc/>
		public int RejectionCount { get; set; }

		/// <inheritdoc/>
		public DateTime? LastAcceptedDate { get; set; }

		/// <inheritdoc/>
		public DateTime? LastAbandonedDate { get; set; }

		/// <inheritdoc/>
		public int AbandonedCount { get; set; }

		/// <inheritdoc/>
		public DateTime? FirstMessageDate { get; set; }

		/// <inheritdoc/>
		public int? FirstResponseTime { get; set; }

		/// <inheritdoc/>
		public DateTime? ResolvedDate { get; set; }

		/// <inheritdoc/>
		public int ReopenedCount { get; set; }

		/// <inheritdoc/>
		public DateTime? ClosedDate { get; set; }

		/// <inheritdoc/>
		public ClosedBy? ClosedBy { get; set; }

		/// <inheritdoc/>
		public int? Csat { get; set; }

		/// <inheritdoc/>
		public DateTime? CsatDate { get; set; }

		/// <inheritdoc/>
		public string Metadata { get; set; }

		/// <inheritdoc/>
		public string BrandName { get; set; }

		/// <inheritdoc/>
		public int? TotalHandlingTime { get; set; }

		/// <inheritdoc/>
		public int? TotalResolutionTime { get; set; }

		/// <inheritdoc/>
		public TicketStatusEnum Status { get; set; }

		/// <inheritdoc/>
		public string LastRejectionReasonsNames { get; set; }

		/// <inheritdoc/>
		public int? Complexity { get; set; }

		/// <inheritdoc/>
		public string LastAbandonmentReasonsNames { get; set; }

		/// <inheritdoc/>
		public string TicketSource { get; set; }

		/// <inheritdoc/>
		public string L1Tags { get; set; }

		/// <inheritdoc/>
		public string L2Tags { get; set; }

		/// <inheritdoc/>
		public string Diagnosis { get; set; }

		/// <inheritdoc/>
		public int? Nps { get; set; }

		/// <inheritdoc/>
		public int? ChaserEmails { get; set; }

		/// <inheritdoc/>
		public int? WaitingTimeL1 { get; set; }

		/// <inheritdoc/>
		public int? WaitingTimeL2 { get; set; }

		/// <inheritdoc/>
		public int? SuperSolverFirstResponse { get; set; }

		/// <inheritdoc/>
		public string CustomerQuery { get; set; }

		/// <inheritdoc/>
		public string CSATFirstResponse
		{
			get
			{
				//if there's no closing date, or if there's no CsatDate, return empty string
				if (!this.ClosedDate.HasValue || !this.CsatDate.HasValue)
					return string.Empty;

				//try and calculate a timespan between closing date and csat date
				TimeSpan? ts = this.CsatDate - this.ClosedDate;

				//if the timespan is less than 60 seconds, return 0. Otherwise, return minutes, converted to int
				return $"{Convert.ToInt32(ts?.TotalMinutes)}";
			}
		}

		/// <inheritdoc/>
		public string CurrentTicketLevel { get; set; }

		/// <inheritdoc/>
		public decimal Price { get; set; }

		/// <inheritdoc/>
		public DateTime? NpsDate { get; set; }

		/// <inheritdoc/>
		public DateTime? DateEscalatedToL2 { get; set; }

		/// <inheritdoc/>
		public string LevelClosed { get; set; }

		/// <inheritdoc/>
		public int RepeatExisting { get; set; }

		/// <inheritdoc/>
		public string RiskCriteria { get; set; }

		/// <inheritdoc/>
		public string RiskLevel { get; set; }

		/// <inheritdoc/>
		public string FraudConfirmed { get; set; }

		/// <inheritdoc/>
		public string SalesLead { get; set; }

		/// <inheritdoc/>
		public string EscalatedBy { get; set; }

		/// <inheritdoc/>
		public string L1SubTags { get; set; }

		/// <inheritdoc/>
		public string L2SubTags { get; set; }

		/// <inheritdoc/>
		public string ProbingQuestions { get; set; }

		/// <inheritdoc/>
		public int SolverResponse { get; set; }

		/// <inheritdoc/>
		public int CustomerResponse { get; set; }

		/// <inheritdoc/>
		public int AvgResponseTime { get; set; }

		/// <inheritdoc/>
		public string RatingComment { get; set; }

		/// <inheritdoc/>
		public string IssueType { get; set; }

		/// <inheritdoc/>
		public int? LongestTime { get; set; }

		/// <inheritdoc/>
		public string InScopeForCrowd { get; set; }
		
		/// <inheritdoc/>
		public string RepeatedInL1 { get; set; }

		/// <inheritdoc/>
		public string RepeatedInL2 { get; set; }
    }
}