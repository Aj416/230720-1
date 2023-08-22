using System;
using System.Collections.Generic;
using System.Linq;
using Tigerspike.Solv.Core.Constants;
using IdGen;
using Tigerspike.Solv.Core.Exceptions;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Core.Extensions;
using System.Globalization;

namespace Tigerspike.Solv.Domain.Models
{
	public class Ticket
	{
		/// <summary>
		/// The minimum value that CSAT can be set to.
		/// </summary>
		public const int MIN_CSAT = 1;

		/// <summary>
		/// The Maximum value that CSAT can be set to.
		/// </summary>
		public const int MAX_CSAT = 5;

		/// <summary>
		/// The minimum value that Complexity rating can be set to.
		/// </summary>
		public const int MIN_COMPLEXITY = 0;

		/// <summary>
		/// The Maximum value that Complexity rating can be set to.
		/// </summary>
		public const int MAX_COMPLEXITY = 10;

		/// <summary>
		/// The minimum value that NPS can be set to.
		/// </summary>
		public const int MIN_NPS = 0;

		/// <summary>
		/// The Maximum value that NPS can be set to.
		/// </summary>
		public const int MAX_NPS = 10;

		// How many times the ticket can be abandoned before get considered critical.
		public const int ABANDON_CRTICIAL_COUNT = 2;

		// How many times the ticket can be rejected before get considered critical.
		public const int REJECT_CRTICIAL_COUNT = 2;

		/// <summary>
		/// The number of days after closing a ticket, the customer is allowed to rate it.
		/// </summary>
		public const int CSAT_ALLOWED_DAYS = 7;

		/// <summary>
		/// How many minutes a ticket can be reserved if it was never abandoned.
		/// </summary>
		public const int NormalReserveExpiryMin = 1;

		/// <summary>
		/// How many minutes a ticket can be reserved if it was abandoned at least once.
		/// </summary>
		public const int UrgentReserveExpiryMin = 3;

		/// <summary>
		/// Diagnosis Correct value
		/// </summary>
		public const string DiagnosisCorrect = "Correct";

		/// <summary>
		/// Diagnosis Incorrect value
		/// </summary>
		public const string DiagnosisIncorrect = "Incorrect";

		/// <summary>
		/// Time before the open ticket is considered critical.
		/// </summary>
		public static readonly TimeSpan CriticalTicketTimeThreshold = new TimeSpan(0, 10, 0);

		public Ticket(User customer, string question, Brand brand, decimal price, decimal fee, string referenceId, string threadId,
			TicketSource source, string culture, TicketTransportType transportType, TicketLevel level, Guid? practicingAdvocateId, IEnumerable<TicketMetadataItem> metadata, IReadOnlyDictionary<Guid, Guid?> probingAnswers, DateTime utcTimeStamp)
		{
			Id = Guid.NewGuid();
			Number = new IdGenerator(0).CreateId();
			Customer = customer;
			Question = question;
			Brand = brand;
			BrandId = brand.Id;
			Price = price;
			Fee = fee;
			ReferenceId = referenceId;
			ThreadId = threadId;
			SourceId = source?.Id;
			Culture = culture;
			TransportType = transportType;
			Level = level;
			StatusHistory = new List<TicketStatusHistory>();
			TrackingHistory = new List<TrackingDetail>();
			Tags = new List<TicketTag>();
			ProbingAnswers = probingAnswers?.Select(x => new ProbingResult(Id, x.Key, x.Value)).ToList();

			Metadata = metadata?.ToList() ?? new List<TicketMetadataItem>();
			Metadata.ForEach(x => x.TicketId = Id);

			CreatedDate = utcTimeStamp;
			ModifiedDate = utcTimeStamp;

			// if practicingAdvocateId has a value, this is a practice ticket and it should be linked
			// to the advocate immediately.
			AdvocateId = practicingAdvocateId;
			IsPractice = practicingAdvocateId != null;
			SetStatus(TicketStatusEnum.New, practicingAdvocateId, utcTimeStamp);
		}

		/// <summary>
		/// Import ticket consutrctor
		/// </summary>
		public Ticket(User customer, string question, Brand brand, decimal price, decimal fee, string referenceId,
			TicketSource source, Guid ticketImportId, IEnumerable<TicketMetadataItem> metadata, DateTime utcTimeStamp) :
			this(customer, question, brand, price, fee, referenceId, null, source, null, TicketTransportType.Import, TicketLevel.Regular, null, metadata, null, utcTimeStamp)
		{
			Brand = brand;
			TicketImportId = ticketImportId;
		}

		/// <summary>
		/// Constructor to please EF.
		/// </summary>
		public Ticket() { }

		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The ticket number.
		/// </summary>
		public long Number { get; set; }

		/// <summary>
		/// Concurrency token
		/// </summary>
		public byte[] RowVersion { get; set; }

		/// <summary>
		/// The question asked by the customer in this ticket.
		/// </summary>
		public string Question { get; set; }

		/// <summary>
		/// Level of the ticket
		/// </summary>
		public TicketLevel Level { get; set; }

		/// <summary>
		/// The complexity value submitted by advocate who solved it.
		/// </summary>
		public int? Complexity { get; set; }

		/// <summary>
		/// The NPS (Net Promoter Score) value of the ticket.
		/// </summary>
		public int? Nps { get; set; }

		/// <summary>
		/// The timestamp of capturing the NPS for the ticket
		/// </summary>
		public DateTime? NpsDate { get; set; }

		/// <summary>
		/// The CSAT submitted by the customer after closing the ticket.
		/// </summary>
		public int? Csat { get; set; }

		/// <summary>
		/// The timestamp of capturing the CSAT for the ticket
		/// </summary>
		public DateTime? CsatDate { get; set; }

		/// <summary>
		/// The price of this ticket.
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// The fee for this ticket.
		/// </summary>
		public decimal Fee { get; set; }

		/// <summary>
		/// The total amount to be paid for the ticket
		/// </summary>
		public decimal Total => Price + Fee;

		/// <summary>
		/// The number of how many times the ticket was abandoned by an advocate. It helps filtering
		/// ticket that are urgent (abandoned at least once).
		/// </summary>
		public int AbandonedCount { get; set; }

		/// <summary>
		/// The number of how many times the ticket was rejected by an advocate. It helps filtering
		/// ticket that are critical.
		/// </summary>
		public int RejectionCount { get; set; }

		/// <summary>
		/// The status of the ticket.
		/// </summary>
		public TicketStatusEnum Status { get; set; }

		/// <summary>
		/// The model of currenlty assigned advocate.
		/// </summary>
		public Guid? AdvocateId { get; set; }

		/// <summary>
		/// The advocate who is currently assigned.
		/// </summary>
		public Advocate Advocate { get; set; }

		/// <summary>
		/// The information of the customer who created this ticket.
		/// </summary>
		public User Customer { get; set; }

		/// <summary>
		/// Is the ticket for practicing.
		/// </summary>
		public bool IsPractice { get; set; }

		/// <summary>
		/// Indicates if the ticket is ready to be pulled by solver after intial creation
		/// </summary>

		public bool Ready { get; set; }

		/// <summary>
		/// The brand that this ticket belongs to.
		/// </summary>
		public Brand Brand { get; set; }

		/// <summary>
		/// The brand that this ticket belongs to.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <inheritdoc/>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The last modification date of the ticket.
		/// </summary>
		public DateTime ModifiedDate { get; set; }

		/// <summary>
		/// The date time of the first message in the conversation after the ticket has been opened.
		/// </summary>
		public DateTime? FirstMessageDate { get; private set; }

		/// <summary>
		/// The date time of the first message of Super solver in the conversation after the ticket has been Assigned to Super Solver
		/// </summary>
		public DateTime? SuperSolverFirstMessageDate { get; private set; }

		/// <summary>
		/// The date of the last message received from the customer (if any)
		/// </summary>
		public DateTime? LastCustomerMessageDate { get; private set; }

		/// <summary>
		/// The start date of the last chunk of customer-solver chat
		/// </summary>
		public DateTime? CurrentCustomerQueryDate { get; private set; }

		/// <summary>
		/// The date of the last message received from the advocate (if any)
		/// </summary>
		public DateTime? LastAdvocateMessageDate { get; private set; }

		/// <summary>
		/// The date time the first time ever the ticket was assigned.
		/// </summary>
		public DateTime? FirstAssignedDate { get; private set; }

		/// <summary>
		/// The date time the ticket was last assigned.
		/// </summary>
		public DateTime? AssignedDate { get; private set; }

		/// <summary>
		/// The date time the ticket was closed.
		/// </summary>
		public DateTime? ClosedDate { get; set; }


		/// <summary>
		/// Solver response time in seconds
		/// </summary>
		public int? SolverTotalResponseTimeInSeconds { get; private set; }

		/// <summary>
		/// Solver response time in seconds
		/// </summary>
		public int? SolverMaxResponseTimeInSeconds { get; private set; }

		/// <summary>
		/// Solver response count
		/// </summary>
		public int SolverResponseCount { get; private set; }

		/// <summary>
		/// Solver Average response time
		/// </summary>
		public int? AverageSolverResponseTime => SolverResponseCount > 0 ? (int?)(SolverTotalResponseTimeInSeconds / SolverResponseCount) : null;

		/// <summary>
		/// Solver all messages count
		/// </summary>
		public int SolverMessageCount { get; private set; }

		/// <summary>
		/// Customer all messages count
		/// </summary>
		public int CustomerMessageCount { get; private set; }

		/// <summary>
		/// Returns the last reservation expiry date if exists.
		/// </summary>
		public DateTime? ReservationExpiryDate { get; set; }

		/// <summary>
		/// The type of the transport for the ticket
		/// </summary>
		public TicketTransportType TransportType { get; set; }

		/// <summary>
		/// Is the ticket considered as critical (based on few conditions)
		/// </summary>
		public bool IsCritical => AbandonedCount >= ABANDON_CRTICIAL_COUNT || RejectionCount >= REJECT_CRTICIAL_COUNT;

		/// <summary>
		/// Was the ticket abandoned before.
		/// </summary>
		public bool IsAbandoned => AbandonedCount > 0;

		/// <summary>
		/// Returns how long (in seconds) the ticket was opened since it was first assigned to a first solver
		/// </summary>
		public int? AbsoluteTimeToClose => (int?)(ClosedDate - FirstAssignedDate)?.TotalSeconds;

		/// <summary>
		/// Returns how long (in seconds) the ticket was opened since it was first assigned to a last (final) solver
		/// </summary>
		public int? SolverTimeToClose => (int?)(ClosedDate - AssignedDate)?.TotalSeconds;

		/// <summary>
		/// The full name of the current assigned advocate.
		/// </summary>
		public string AdvocateFullName => Advocate?.User?.FullName;

		/// <summary>
		/// The client invoice id if this ticket was invoiced.
		/// </summary>
		public Guid? ClientInvoiceId { get; set; }

		/// <summary>
		/// The advocate invoice id if this ticket was invoiced.
		/// </summary>
		public Guid? AdvocateInvoiceId { get; set; }

		/// <summary>
		/// External system reference id (for integration purposes)
		/// </summary>
		public string ReferenceId { get; set; }

		/// <summary>
		/// The thread id in the messenger conversation.
		/// </summary>
		public string ThreadId { get; set; }

		/// <summary>
		/// The source reference id that created this ticket
		/// </summary>
		public int? SourceId { get; set; }

		/// <summary>
		/// The source that created this ticket
		/// </summary>
		public TicketSource Source { get; set; }

		/// <summary>
		/// Reason why ticket has been escalated
		/// </summary>
		public TicketEscalationReason? EscalationReason { get; set; }

		/// <summary>
		/// Timestamp of escalation
		/// </summary>
		public DateTime? EscalatedDate { get; set; }

		private IEnumerable<AbandonReason> AbandonReasons => AbandonHistory?.Count() > 0
			? AbandonHistory
				.OrderBy(x => x.CreatedDate)
				.Select(x => x.Reasons)
				.LastOrDefault()
				.Select(x => x.AbandonReason)
			: null;

		/// <summary>
		/// Solver that triggered the escalation of the ticket
		/// </summary>
		public Guid? EscalatedById { get; set; }

		/// <summary>
		/// The advocate who escalated the ticket
		/// </summary>
		public Advocate EscalatedSolver { get; set; }

		/// <summary>
		/// Identify who asked to close a ticket.
		/// </summary>
		public ClosedBy? ClosedBy { get; set; }

		/// <summary>
		/// Whether or not escalated ticket was diagnosed correctly by a regular solver
		/// </summary>
		public bool? CorrectlyDiagnosed { get; set; }

		/// <summary>
		/// State of the "Returning Customer" flow
		/// </summary>
		public ReturningCustomerState ReturningCustomerState { get; set; }

		/// <summary>
		/// State of the resuming the chat from link in notification
		/// </summary>
		public NotificationResumptionState NotificationResumptionState { get; set; }

		/// <summary>
		/// Fraud status on the ticket
		/// </summary>
		public FraudStatus FraudStatus { get; set; }

		/// <summary>
		/// List of probing form answers
		/// </summary>
		public ICollection<ProbingResult> ProbingAnswers { get; set; }

		/// <summary>
		/// Diagnosis of ticket
		/// </summary>
		public string Diagnosis => (CorrectlyDiagnosed) switch
		{
			true => DiagnosisCorrect,
			false => DiagnosisIncorrect,
			null => null
		};

		/// <summary>
		/// Whether ticket contains SPOS lead or not
		/// </summary>
		public bool? SposLead { get; set; }

		/// <summary>
		/// Verbose details about SPOS lead
		/// </summary>
		public string SposDetails { get; set; }

		/// <summary>
		/// Ticket culture
		/// </summary>
		public string Culture { get; set; }

		public CultureInfo CultureInfo => Culture != null ? new CultureInfo(Culture) : null;

		/// <summary>
		/// Number of chaser emails have been sent to the Customer
		/// </summary>
		public int? ChaserEmails { get; set; }

		/// <summary>
		/// Key to import process
		/// </summary>
		public Guid? TicketImportId { get; set; }

		/// <summary>
		/// Import process associated with that ticket
		/// </summary>
		public TicketImport TicketImport { get; set; }

		/// <summary>
		/// The list of all statuses that this ticket went through.
		/// </summary>
		public List<TicketStatusHistory> StatusHistory { get; set; }

		/// <summary>
		/// The list of all rejection reasons that the advocate specified when rejecting it.
		/// </summary>
		public List<TicketRejectionHistory> RejectionHistory { get; set; }

		/// <summary>
		/// The tags of the ticket
		/// </summary>
		public ICollection<TicketTag> Tags { get; set; }

		/// <summary>
		/// The current tagging status of the ticket
		/// </summary>
		public TicketTagStatus? TagStatus { get; set; }

		/// <summary>
		/// The tracking details of the ticket
		/// </summary>
		public ICollection<TrackingDetail> TrackingHistory { get; set; }

		/// <summary>
		/// State of Fraud Risk.
		/// Can be one of Low, Medium, High.
		/// </summary>
		public RiskLevel FraudRiskLevel { get; set; }

		/// <summary>
		/// Comma separated list of all risks.
		/// </summary>
		public string FraudRisks { get; set; }

		/// <summary>
		/// Flat, formatted last rejection reasons
		/// </summary>
		public string LastRejectionReasonsNames => RejectionHistory?.Count() > 0
			? RejectionHistory
				.OrderBy(x => x.CreatedDate)
				.Select(x => x.Reasons)
				.LastOrDefault()
				.Select(x => x.RejectionReason.Name)
				.Concatenate("|", null)
			: null;

		/// <summary>
		/// The list of all Abandon reasons that the advocate specified when Abandon it.
		/// </summary>
		public List<TicketAbandonHistory> AbandonHistory { get; set; }

		/// <summary>
		/// Determines if SPOS email was sent.
		/// </summary>
		public bool? SposEmailSent { get; set; }

		/// <summary>
		/// Formatted last Abandon reasons names
		/// </summary>
		public string LastAbandonmentReasonsNames => AbandonHistory?.Count() > 0
			? AbandonHistory
				.OrderBy(x => x.CreatedDate)
				.Select(x => x.Reasons)
				.LastOrDefault()
				.Select(x => x.AbandonReason.Name)
				.Concatenate("|", null)
			: null;

		/// <summary>
		/// The list of passed in metadata items while creating ticket
		/// </summary>
		public List<TicketMetadataItem> Metadata { get; set; }

		/// <summary>
		/// Gets or sets category of ticket
		/// </summary>
		public TicketCategory TicketCategory { get; set; }

		/// <summary>
		/// Whether or not escalated ticket was valid transfer by a regular solver
		/// </summary>
		public bool? ValidTransfer { get; set; }

		/// <summary>
		/// Customer feedback relevant to support.
		/// </summary>
		public string AdditionalFeedBack { get; set; }

		/// <summary>
		/// Repeted customer count L1
		/// </summary>
		public int RepeatedInL1 { get; set; }

		/// <summary>
		/// Repeted customer count L2
		/// </summary>
		public int RepeatedInL2 { get; set; }

		/// <summary>
		/// Determines if diagnosis ie required.
		/// </summary>
		public bool IsDiagnosisRequired
		{
			get
			{
				var isDiagnosisDisabled = Tags?.Where(tt => tt.Level == TicketLevel.Regular).Select(tt => tt.Tag)?.Where(t => t.DiagnosisEnabled == false).Count() > 0;
				return (EscalatedById, isDiagnosisDisabled) switch
				{
					(null, true) => false,
					(null, false) => false,
					(_, true) => false,
					(_, _) => true
				};
			}
		}

		/// <summary>
		/// Flat, formatted ticket metadata
		/// </summary>
		public string FlatMetadata => Metadata
			.Select(x => $"{x.Key}:{x.Value}")
			.Concatenate("|", null);

		/// <summary>
		/// Last date when a solver Resolve a ticket, only when ticket is closed
		/// </summary>
		public DateTime? ResolvedDate
		{
			get
			{
				// we return CreatedDate from the last record StatusHistory with Status Solved, only when ticket is closed
				var history = StatusHistory.OrderBy(x => x.CreatedDate).ToList();
				return history?.Zip(history?.Skip(1))
					.Where(x => x.First.Status == TicketStatusEnum.Solved && x.Second.Status == TicketStatusEnum.Closed)
					.Select(x => x.First.CreatedDate as DateTime?)
					.LastOrDefault();
			}
		}

		/// <summary>
		/// Last date when a solver Abandoned a ticket
		/// </summary>
		public DateTime? LastAbandonedDate
		{
			get
			{
				// we return CreatedDate from the last record StatusHistory with Status New/Escalated, only when ticket is Abandoned
				var history = StatusHistory.OrderBy(x => x.CreatedDate).ToList();
				return history?.Zip(history?.Skip(1))
					.Where(x => x.First.Status == TicketStatusEnum.Assigned && x.Second.Status.IsIn(TicketStatusEnum.New, TicketStatusEnum.Escalated) && AbandonHistory.Any(a => a.CreatedDate == x.Second.CreatedDate))
					.Select(x => x.Second.CreatedDate as DateTime?)
					.LastOrDefault();
			}
		}

		/// <summary>
		/// Last date when a solver accepted a ticket for solving
		/// </summary>
		public DateTime? LastAcceptedDate
		{
			get
			{
				// we return CreatedDate from the last record StatusHistory with Status Assigned than is immediately preceeded by Status Reserved
				var history = StatusHistory.OrderBy(x => x.CreatedDate).ToList();
				return history?.Zip(history?.Skip(1))
					.Where(x => x.First.Status == TicketStatusEnum.Reserved && x.Second.Status == TicketStatusEnum.Assigned)
					.Select(x => x.Second.CreatedDate as DateTime?)
					.LastOrDefault();
			}
		}

		/// <summary>
		/// First date when a regular solver accepted a ticket.
		/// </summary>
		public DateTime? FirstRegularSolverAcceptedDate => StatusHistory
			.Where(x => x.Status == TicketStatusEnum.Assigned)
			.Where(x => x.Level == TicketLevel.Regular)
			.OrderBy(x => x.CreatedDate)
			.Select(x => (DateTime?)(x.CreatedDate)) // cast as null, as there might not be any item matching criteria
			.FirstOrDefault();

		/// <summary>
		/// First date when a super solver accepted a ticket.
		/// </summary>
		public DateTime? FirstSuperSolverAcceptedDate
		{
			get
			{
				// we return CreatedDate from the last record StatusHistory with Status Escalated than is immediately preceeded by Status Assigned
				var history = StatusHistory.OrderBy(x => x.CreatedDate).ToList();
				var firstIndexofEscalation = history.IndexOf(history.Where(x => x.Status == TicketStatusEnum.Escalated).FirstOrDefault());

				return firstIndexofEscalation > default(int) ? history.Skip(firstIndexofEscalation).Where(x => x.Status == TicketStatusEnum.Assigned).Select(x => x.CreatedDate as DateTime?).FirstOrDefault() : null;
			}
		}

		/// <summary>
		/// Last date when a super solver accepted a ticket.
		/// </summary>
		public DateTime? LastSuperSolverAcceptedDate
		{
			get
			{
				// we return CreatedDate from the last record StatusHistory with Status Escalated than is immediately preceeded by Status Assigned
				var history = StatusHistory.OrderBy(x => x.CreatedDate).ToList();
				var lastIndexofEscalation = history.IndexOf(history.Where(x => x.Status == TicketStatusEnum.Escalated).LastOrDefault());

				return lastIndexofEscalation > default(int) ? history.Skip(lastIndexofEscalation).Where(x => x.Status == TicketStatusEnum.Assigned).Select(x => x.CreatedDate as DateTime?).FirstOrDefault() : null;
			}
		}

		/// <summary>
		/// The super solver who addressed escalated the ticket
		/// </summary>
		public Advocate SuperSolver
		{
			get
			{
				var history = StatusHistory.Where(x => x.Level == TicketLevel.SuperSolver).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
				return history?.Advocate;
			}
		}

		/// <summary>
		/// Advocate name that was last to reject ticket
		/// </summary>
		public IEnumerable<string> RejectedBy
		{
			get
			{
				// we return Advocate from the last record in that list with Status Reserved that is immediately followed by Status New/Escalated (when data is sorted by CreatedDate)
				var history = StatusHistory.OrderBy(x => x.CreatedDate).ToList();
				return history.Zip(history.Skip(1))
					.Where(x => x.First.Status == TicketStatusEnum.Reserved &&
								x.Second.Status.IsIn(TicketStatusEnum.New, TicketStatusEnum.Escalated))
					.Select(x => x.First.Advocate.User.FullName);
			}
		}

		/// <summary>
		/// Add a new column 'CurrentTicketLevel'
		/// Show L1 if the current solver is L1, excluding tickets which L1 has escalated to L2
		/// Show L2 if currently assigned to L2, including tickets escalated from L1
		/// </summary>
		public string CurrentTicketLevel => Advocate != null ? Advocate.Super ? "L2" : EscalatedById != null ? "L2" : "L1" : string.Empty;

		/// <summary>
		/// Advocate name that was last to reject ticket
		/// </summary>
		public string LastRejectedBy => RejectedBy.LastOrDefault();

		/// <summary>
		/// Advocate name that was last to Abandon ticket
		/// </summary>
		public string LastAbandonedBy
		{
			get
			{
				// we need to return Advocate from the last record in that list with Status Assigned that is immediately followed by Status New/Escalated (when data is sorted by CreatedDate)
				var history = StatusHistory.OrderBy(x => x.CreatedDate).ToList();
				return history?.Zip(history?.Skip(1))
					.Where(x => x.First.Status == TicketStatusEnum.Assigned && x.Second.Status.IsIn(TicketStatusEnum.New, TicketStatusEnum.Escalated) && AbandonHistory.Any(a => a.CreatedDate == x.Second.CreatedDate))
					.Select(x => x.First.Advocate.User.FullName)
					.LastOrDefault();
			}
		}

		/// <summary>
		/// Advocate name that was last to Accept the ticket
		/// </summary>
		public string LastAcceptedBy
		{
			get
			{
				// we return Advocate from the last record in that list with Status Reserved that is immediately followed by Status Assigned (when data is sorted by CreatedDate)
				var history = StatusHistory.OrderBy(x => x.CreatedDate).ToList();
				return history?.Zip(history?.Skip(1))
					.Where(x => x.First.Status == TicketStatusEnum.Reserved && x.Second.Status == TicketStatusEnum.Assigned)
					.Select(x => x.First.Advocate.User.FullName)
					.LastOrDefault();
			}
		}

		/// <summary>
		/// Super Solver first response when ticket is accepted by Super Solver
		/// </summary>
		public int? SuperSolverFirstResponse => (int?)(SuperSolverFirstMessageDate - FirstSuperSolverAcceptedDate)?.TotalMinutes;

		/// <summary>
		/// waiting time for L2 from when a ticket is Escalated until Assigned to L2
		/// </summary>
		public int? WaitingTimeL2 => (int?)(FirstSuperSolverAcceptedDate - EscalatedDate)?.TotalMinutes;

		/// <summary>
		/// How many times closed ticket was reopened during lifetime
		/// </summary>
		public int ReopenedCount
		{
			get
			{
				var history = StatusHistory.OrderBy(x => x.CreatedDate).ToList();
				return Status == TicketStatusEnum.Closed ? history.Zip(history.Skip(1))
					.Where(x => x.First.Status == TicketStatusEnum.Solved && x.Second.Status == TicketStatusEnum.Assigned)
					.Count() : 0;
			}
		}

		// public int ReopenedCount => Status == TicketStatusEnum.Closed ? StatusHistory.Count(x => x.Status == TicketStatusEnum.Solved) - 1 : 0;

		/// <summary>
		/// Gets how the ticket was escalated and if the ticket moved from level 1 to level 2 due to level 1 action
		/// </summary>
		public string EscalatedBy
		{
			get
			{
				if (EscalatedById.HasValue)
				{
					return "L1";
				}

				var firstAbandonReason = AbandonReasons?.FirstOrDefault();

				if (EscalationReason == TicketEscalationReason.AbandonedCountExceeded && firstAbandonReason != null &&
					!AbandonReasons.FirstOrDefault().IsBlockedAdvocate)
				{
					return "L1";
				}

				if (AbandonReasons != null &&
					AbandonReasons.Any(x => x.Action == TicketFlowAction.Escalate))
				{
					return "L1";
				}

				if (EscalationReason == TicketEscalationReason.AdminEscalated)
				{
					return "Admin";
				}

				if (firstAbandonReason != null && (firstAbandonReason.IsAutoAbandoned || firstAbandonReason.IsBlockedAdvocate) &&
					EscalationReason == TicketEscalationReason.AbandonedCountExceeded)
				{
					return "Admin";
				}

				if (EscalationReason == TicketEscalationReason.Customer)
				{
					return "WA";
				}

				return string.Empty;
			}
		}

		/// <summary>
		/// Total resolution time when ticket is closed
		/// </summary>
		public int? TotalResolutionTime => (int?)(ResolvedDate - CreatedDate)?.TotalMinutes;

		/// <summary>
		/// Total handling time when ticket is closed
		/// </summary>
		public int? TotalHandlingTime => (int?)(ClosedDate - CreatedDate)?.TotalMinutes;

		/// <summary>
		/// Time passed between creation of the ticket and first Solver's response
		/// </summary>
		public int? FirstResponseTime => (int?)(FirstMessageDate - CreatedDate)?.TotalMinutes;

		/// <summary>
		/// Waiting time for L1 solver
		/// </summary>
		public int? WaitingTimeL1 => (int?)(FirstRegularSolverAcceptedDate - CreatedDate)?.TotalMinutes;

		/// <summary>
		/// Determines if valid transfer is required.
		/// </summary>
		public bool IsValidTransferRequired
		{
			get
			{
				return (Level, Brand?.ValidTransferEnabled) switch
				{
					(TicketLevel.SuperSolver, true) => true,
					(_, _) => false
				};
			}
		}

		/// <summary>
		/// Determines if pending status web socket needs to be sent.
		/// </summary>
		public bool IsPendingStatusNotificationRequired
		{
			get
			{
				var flag = Tags?
						.Select(x => x.Tag)
						.Where(x => x.Action.HasValue && x.Action == TicketFlowAction.Escalate && (x.DiagnosisEnabled ?? true))
						.Any() ?? false;
				return flag && EscalationReason == TicketEscalationReason.Tag;
			}
		}


		public void SetSpos(bool lead, string details) => (SposLead, SposDetails) = (lead, details);

		/// <summary>
		/// Set the ticket as reserved to the passed advocate id.
		/// </summary>
		/// <param name="advocateId">The advocate id to be reserved to.</param>
		/// <param name="utcTimeStamp">The UTC timestamp.</param>
		public void Reserve(Guid advocateId, DateTime utcTimeStamp)
		{
			// Set the reservation expiry date.
			ReservationExpiryDate = utcTimeStamp.AddMinutes(AbandonedCount == 0 ? NormalReserveExpiryMin : UrgentReserveExpiryMin);
			AdvocateId = advocateId;

			SetStatus(TicketStatusEnum.Reserved, advocateId, utcTimeStamp);
		}

		public void CancelReservation()
		{
			Reject(
				new[] { RejectionReason.ReservationExpiredReasonId }, // mark special rejection reason
				new FixedTimestampService(ReservationExpiryDate.Value).GetUtcTimestamp() // mark rejection in that particular point in time
			);
		}

		/// <summary>
		/// The current advocate (who reserved the ticket) has accepted it Now the ticket will be
		/// assigned to him.
		/// <param name="utcTimestamp">The UTC timestamp.</param>
		/// </summary>
		public void Accept(DateTime utcTimestamp)
		{
			if (Status != TicketStatusEnum.Reserved || utcTimestamp > ReservationExpiryDate)
			{
				throw new DomainException("The ticket is not in a reserved state or reservation expired");
			}

			AssignedDate = utcTimestamp;
			// If this is the first time to assign a ticket, set the first assigned date field.
			FirstAssignedDate = FirstAssignedDate ?? AssignedDate;

			SetStatus(TicketStatusEnum.Assigned, AdvocateId.Value, utcTimestamp);
		}

		/// <summary>
		/// The current advocate (who reserved the ticket) has rejected it Now the ticket will be
		/// returned to New status.
		/// </summary>
		public void Reject(int[] rejectReasonIds, DateTime utcTimestamp)
		{
			var lastStatus = StatusHistory.OrderBy(sh => sh.CreatedDate).Last();
			if (lastStatus.Status != TicketStatusEnum.Reserved)
			{
				throw new DomainException($"Ticket {Id} is not in a reserved state or reservation expired.");
			}

			RejectionCount++;
			// Add all the rejection reasons to the list.
			RejectionHistory.Add(new TicketRejectionHistory(Id, lastStatus.Id, rejectReasonIds, utcTimestamp));

			// If the ticket is practice ticket, the advocate should still see it again and thus, he/she should still be linked to it.
			AdvocateId = IsPractice ? AdvocateId : null;
			ReservationExpiryDate = null;

			SetStatus(TicketStatusEnum.New, AdvocateId, utcTimestamp);
		}

		/// <summary>
		/// The current advocate has maked this ticket as solved. Next, the customer should close it
		/// or reopen it.
		/// </summary>
		public void Solve(DateTime utcTimestamp)
		{
			if (Status != TicketStatusEnum.Assigned)
			{
				throw new DomainException($"Ticket {Id} is not in Assigned status.");
			}

			if (Brand.TagsEnabled && Tags.Count == 0)
			{
				throw new DomainException($"Ticket {Id} does not have any tags assigned to it.");
			}

			if (Brand.CategoryEnabled && TicketCategory == null && TransportType != TicketTransportType.Import)
			{
				throw new DomainException($"Ticket {Id} does not have any category assigned to it.");
			}

			if (EscalatedById == null && CorrectlyDiagnosed != null)
			{
				throw new DomainException($"Ticket {Id} was not escalated by L1 solver, so assigning diagnosis result is not valid operation.");
			}

			if (IsValidTransferRequired)
			{
				if (Level != TicketLevel.SuperSolver && ValidTransfer != null)
				{
					throw new DomainException($"Ticket {Id} was not escalated, so assigning valid transfer is not valid operation.");
				}

				if (Level == TicketLevel.SuperSolver && ValidTransfer == null)
				{
					throw new DomainException($"Ticket {Id} does not have valid transfer assigned to it.");
				}
			}

			SetStatus(IsPractice ? TicketStatusEnum.Closed : TicketStatusEnum.Solved, AdvocateId.Value, utcTimestamp);
		}

		public bool AreTagsAvailable()
		{
			return Brand.TagsEnabled || Brand.CategoryEnabled || Brand.SposEnabled;
		}

		public bool IsMinTagged()
		{
			List<bool> taggingStatuses = GetTaggingStatuses();
			return taggingStatuses.Any(x => x);
		}

		public bool IsTaggingComplete()
		{
			var taggingStatuses = GetTaggingStatuses();
			return taggingStatuses.All(x => x);
		}

		public void SetReady() => Ready = true;
		
		public void SetTags(Guid[] tagIds, TicketLevel level, DateTime? utcTimestamp)
		{
			Tags ??= new List<TicketTag>();

			// Clear previous tags for level
			var tagsForLevel = Tags.Where(t => t.Level == level).ToList();
			var tagsToUpdate = tagsForLevel.Where(t => tagIds.Contains(t.TagId)).ToList();
			var tagsToRemove = tagsForLevel.Where(t => !tagIds.Contains(t.TagId)).ToList();

			foreach (var ticketTag in tagsToRemove)
			{
				Tags.Remove(ticketTag);
			}

			// Since the record does not have a unique key we'd need to update the record instead
			// of removing and inserting a new one
			foreach (var ticketTag in tagsToUpdate)
			{
				// Update ticket tag
				ticketTag.UserId = AdvocateId;
				ticketTag.CreatedDate = utcTimestamp;
			}

			foreach (var tagId in tagIds.Except(tagsToUpdate.Select(t => t.TagId)))
			{
				Tags.Add(new TicketTag(Id, tagId, level, AdvocateId, utcTimestamp));
			}
		}

		/// <summary>
		/// The current advocate can't solve it, and has abandoned it. He/She is not assigned to it
		/// any more. It will return to New status, waiting for another advocate.
		/// </summary>
		public void Abandon(Guid[] abandonReasonIds, decimal latestPrice, decimal fees, DateTime utcTimestamp)
		{
			var lastStatus = StatusHistory.OrderBy(sh => sh.CreatedDate).Last();
			if (lastStatus.Status.IsIn(TicketStatusEnum.Assigned, TicketStatusEnum.Solved))
			{
				AbandonedCount++;
				AbandonHistory.Add(new TicketAbandonHistory(Id, AdvocateId, lastStatus.Id, abandonReasonIds, utcTimestamp));

				Price = latestPrice;
				Fee = fees;
				// If the ticket is practice ticket, the advocate should still see it again and thus, he/she should still be linked to it.
				AdvocateId = IsPractice ? AdvocateId : null;
				SetStatus(TicketStatusEnum.New, AdvocateId, utcTimestamp);
				SetTags(Array.Empty<Guid>(), Level, utcTimestamp); // Remove tags in case ticket is abandoned.
				TicketCategory = null; // reset category in case ticket is abandoned.
				if (IsPractice || SposEmailSent != true)
				{
					SposLead = null; // reset spos lead.
					SposDetails = null; // reset spos details.
				}
			}
			else
			{
				throw new DomainException($"Ticket {Id} cannot be abandoned.");
			}
		}

		public void SetReturningCustomerState(ReturningCustomerState state) => ReturningCustomerState = state;

		public void SetRepeatedCustomerCount()
		{
			if (Level == TicketLevel.Regular)
			{
				RepeatedInL1++;
			}
			else if (Level == TicketLevel.SuperSolver)
			{
				RepeatedInL2++;
			}
		}
		public void SetNotificationResumptionState(NotificationResumptionState state) => NotificationResumptionState = state;

		/// <summary>
		/// The custoemr refused to close it, after the advocate marked it as solved And asked to
		/// reopen it, the last advocate will be assigned again to it.
		/// </summary>
		public void Reopen(DateTime utcTimestamp)
		{
			if (Status != TicketStatusEnum.Solved)
			{
				throw new DomainException($"Ticket {Id} is not in Solved status.");
			}

			// Assign the ticket back to the last advocate.
			SetStatus(TicketStatusEnum.Assigned, AdvocateId.Value, utcTimestamp);
		}

		/// <summary>
		/// The customer asked to close it, after the advocate marked it as solved. The ticket is now
		/// closed.
		/// </summary>
		public void Close(DateTime utcTimestamp, ClosedBy closedBy)
		{
			if (Status == TicketStatusEnum.Closed)
			{
				throw new DomainException($"Ticket {Id} has already been closed and cannot be closed by {closedBy}.");
			}

			ClosedDate = utcTimestamp;
			ClosedBy = closedBy;
			TagStatus = TicketTagStatus.Complete;

			var previousStatus = Status;
			SetStatus(TicketStatusEnum.Closed, AdvocateId, utcTimestamp);

			if (EscalatedById != null) // if escalated from L1, then assign it back if certain conditions are met
			{
				if ((previousStatus == TicketStatusEnum.Solved && CorrectlyDiagnosed == false) || ClosedBy == Enums.ClosedBy.EndChat)
				{
					// remain ticket with current level Solver (>= L2)
					//   no special action required for now, but put the code in that way so it's more clear
					//   what is the ONLY case that requires ticket to remain with L2
				}
				else
				{
					// assign it back to the original solver for CSAT / Complexity and payment
					AdvocateId = EscalatedById;
					Level = TicketLevel.Regular;
				}
			}
		}

		/// <summary>
		/// The custoemr asked to close it, after the advocate marked it as solved. The ticket is now
		/// closed.
		/// </summary>
		public void Escalate(TicketEscalationReason reason, TicketLevel? level, DateTime utcTimeStamp)
		{
			var previousLevel = Level;

			EscalatedDate ??= utcTimeStamp;

			EscalationReason = reason;
			if (EscalatedById == null && reason == TicketEscalationReason.Tag)
			{
				EscalatedById = AdvocateId;
			}

			AdvocateId = null;

			Level = (Brand.SuperSolversEnabled, Brand.PushBackToClientEnabled, level) switch
			{
				(true, _, null) => TicketLevel.SuperSolver,
				(true, _, TicketLevel.SuperSolver) => TicketLevel.SuperSolver,
				(_, true, null) => TicketLevel.PushedBack,
				(_, true, TicketLevel.PushedBack) => TicketLevel.PushedBack,
				(_, _, _) => throw new DomainException($"Illegal escalation combination (SuperSolversEnabled: {Brand.SuperSolversEnabled}, PushBackToClientEnabled: {Brand.PushBackToClientEnabled}, Level: {level})"),
			};

			SetTags(Tags.Where(x => x.Level == previousLevel).Select(x => x.TagId).ToArray(), Level, utcTimeStamp); // copy tags set on the previous level
			SetStatus(TicketStatusEnum.Escalated, AdvocateId, utcTimeStamp);

			if (Level == TicketLevel.SuperSolver)
			{
				SetStatus(TicketStatusEnum.New, AdvocateId, utcTimeStamp); // make ticket availabe to be picked up
			}
		}

		/// <summary>
		/// Sets the value of CSAT submitted by the customer.
		/// </summary>
		public void SetCSAT(int csatScore, DateTime utcTimeStamp)
		{
			if (Status != TicketStatusEnum.Closed)
			{
				throw new DomainException($"Ticket {Id} CSAT can be rated only if it is in closed state.");
			}

			if (csatScore < MIN_CSAT || csatScore > MAX_CSAT)
			{
				throw new DomainException($"Csat score should be between {MIN_CSAT} and {MAX_CSAT}");
			}

			if ((utcTimeStamp - ModifiedDate).TotalDays > CSAT_ALLOWED_DAYS)
			{
				throw new DomainException($"Csat score should be updated within {CSAT_ALLOWED_DAYS} days.");
			}

			Csat = csatScore;
			CsatDate = utcTimeStamp;
		}

		/// <summary>
		/// Sets the value of ticket complexity submitted by the advocate.
		/// </summary>
		public void SetComplexity(int complexity)
		{
			if (Status != TicketStatusEnum.Closed)
			{
				throw new DomainException(($"Ticket {Id} can only set complexity when it is closed"));
			}

			Complexity = complexity;
		}

		/// <summary>
		/// Sets the NPS value for the ticket.
		/// </summary>
		/// <param name="nps"></param>
		/// <exception cref="DomainException"></exception>
		public void SetNps(int nps, DateTime utcTimeStamp)
		{
			if (Status != TicketStatusEnum.Closed)
			{
				throw new DomainException($"Ticket {Id} NPS can be rated only if it is in closed state.");
			}

			if (nps < MIN_NPS || nps > MAX_NPS)
			{
				throw new DomainException(($"NPS value is invalid"));
			}

			Nps = nps;
			NpsDate = utcTimeStamp;
		}

		/// <summary>
		/// Sets the fraud status on the ticket
		/// </summary>
		/// <param name="fraudStatus">New fraud status</param>
		public void SetFraudStatus(FraudStatus fraudStatus) => FraudStatus = fraudStatus;

		/// <summary>
		///
		/// </summary>
		/// <param name="riskLevel"></param>
		/// <param name="risks"></param>
		public void SetFraudDetection(RiskLevel? riskLevel, Dictionary<string, RiskLevel> risks)
		{
			if ((risks == null || risks.Count == 0) && riskLevel == null)
			{
				FraudStatus = FraudStatus.None;
				FraudRiskLevel = riskLevel ?? RiskLevel.None;
				FraudRisks = risks != null ? string.Join(",", risks.Select(x => x.Key)) : null;
			}
			else
			{
				if (FraudStatus == FraudStatus.None)
				{
					FraudStatus = FraudStatus.FraudSuspected;
				}

				FraudRiskLevel = riskLevel ?? RiskLevel.Low;
				FraudRisks = string.Join(",", risks.Select(x => x.Key));
			}
		}

		/// <summary>
		/// Sets the timestamp of ticket last message submitted by advocate or customer
		/// </summary>
		public void SetMessageDatesDetails(DateTime messageDate, int senderType)
		{
			if (senderType == (int)UserType.Advocate || senderType == (int)UserType.SuperSolver)
			{
				FirstMessageDate = FirstMessageDate ?? messageDate;
				LastAdvocateMessageDate = messageDate;
				SolverMessageCount++;
			}

			if (senderType == (int)UserType.SuperSolver)
			{
				SuperSolverFirstMessageDate = SuperSolverFirstMessageDate ?? messageDate;
			}

			if (senderType == (int)UserType.Customer)
			{
				CurrentCustomerQueryDate = CurrentCustomerQueryDate ?? messageDate;
				LastCustomerMessageDate = messageDate;
				CustomerMessageCount++;
			}
		}

		/// <summary>
		/// Sets new pricing on the ticket
		/// </summary>
		/// <param name="price">Price of the ticket</param>
		/// <param name="fee">Ticket's fee</param>
		public void SetPricing(decimal price, decimal fee)
		{
			Price = price;
			Fee = fee;
		}

		/// <summary>
		/// Adds a tracking record to the history based on the user.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="ipAddress">The ip address of the user.</param>
		/// <param name="userAgent">The user agent.</param>
		/// <param name="eventName">The name of the event.</param>
		/// <param name="dateTime">The date time of the event.</param>
		public void Track(User user, string ipAddress, string userAgent, string eventName, DateTime dateTime)
		{
			TrackingHistory.Add(new TrackingDetail
			{
				CreatedDate = dateTime,
				IpAddress = ipAddress,
				TicketId = Id,
				Event = eventName,
				UserAgent = userAgent,
				User = user
			});
		}

		/// <summary>
		/// Sets the status of the ticket and adds a new entry to the status history with a new
		/// status specified.
		/// </summary>
		/// <param name="status">The tickets status value</param>
		/// <param name="advocateId">The advocate id associated with the status if any</param>
		/// <param name="timestampService">The timestamp service provider</param>
		private void SetStatus(TicketStatusEnum status, Guid? advocateId, DateTime utcTimestamp)
		{
			ModifiedDate = utcTimestamp;
			Status = status;
			StatusHistory.Add(new TicketStatusHistory(Id, status, utcTimestamp, advocateId, Level));
		}

		/// <summary>
		/// Returns date when the ticket was accepted by advocate.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <returns>Date when the ticket was accepted by advocate.</returns>
		public DateTime? GetAcceptedDateByAdvocate(Guid? advocateId)
		{
			if (advocateId.HasValue)
			{
				var history = StatusHistory.OrderBy(x => x.CreatedDate).ToList();
				return history.Where(x => x.Status == TicketStatusEnum.Assigned && x.AdvocateId == advocateId).Select(x => x.CreatedDate as DateTime?).LastOrDefault();
			}
			return null;
		}

		/// Returns date when the ticket was accepted by closed.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <returns>Date when the ticket was closed by customer.</returns>
		public DateTime? GetClosedDateForAdvocate(Guid? advocateId)
		{
			if (advocateId.HasValue)
			{
				var history = StatusHistory.OrderBy(x => x.CreatedDate).ToList();
				return history.Where(x => x.Status == TicketStatusEnum.Closed && x.AdvocateId == advocateId).Select(x => x.CreatedDate as DateTime?).LastOrDefault();
			}
			return null;
		}

		/// Returns date when the ticket was solved by customer.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <returns>Date when the ticket was solved by advocate.</returns>
		public DateTime? GetSolvedDateByAdvocate(Guid? advocateId)
		{
			if (advocateId.HasValue)
			{
				var history = StatusHistory.OrderBy(x => x.CreatedDate).ToList();
				return history.Where(x => x.Status == TicketStatusEnum.Solved && x.AdvocateId == advocateId).Select(x => x.CreatedDate as DateTime?).LastOrDefault();
			}
			return null;
		}

		/// <summary>
		/// Sets the solver response time
		/// </summary>
		public void SetSolverResponseTime(DateTime messageDate, int senderType)
		{
			if (senderType == (int)UserType.Advocate || senderType == (int)UserType.SuperSolver)
			{
				var currentResponseTime = messageDate - CurrentCustomerQueryDate;
				if (currentResponseTime != null)
				{
					SolverTotalResponseTimeInSeconds = (SolverTotalResponseTimeInSeconds ?? 0) + (int)currentResponseTime.Value.TotalSeconds;
					SolverMaxResponseTimeInSeconds = Math.Max(SolverMaxResponseTimeInSeconds ?? 0, (int)currentResponseTime.Value.TotalSeconds);
					SolverResponseCount++;
					CurrentCustomerQueryDate = null;
				}
			}

		}

		/// <summary>
		/// Maintanace method only. Can be removed once MaintenanceService.InitializeSolverResponseTime is run on production
		/// </summary>
		/// <param name="solverResponseCount"></param>
		/// <param name="solverTotalResponseTimeInSeconds"></param>
		public void SetSolverResponseDetails(int solverResponseCount, TimeSpan solverTotalResponseTime, TimeSpan solverMaxResponseTime, DateTime? currentCustomerQueryDate)
		{
			SolverResponseCount = solverResponseCount;
			SolverTotalResponseTimeInSeconds = (int)solverTotalResponseTime.TotalSeconds;
			SolverMaxResponseTimeInSeconds = (int)solverMaxResponseTime.TotalSeconds;
			CurrentCustomerQueryDate = currentCustomerQueryDate;
		}

		/// <summary>
		/// Maintanace method only. Can be removed once MaintenanceService.InitializeSolverResponseTime is run on production
		/// </summary>
		/// <param name="solverMessageCount"></param>
		/// <param name="customerMessageCount"></param>
		public void SetMessageCounts(int solverMessageCount, int customerMessageCount)
		{
			SolverMessageCount = solverMessageCount;
			CustomerMessageCount = customerMessageCount;
		}

		/// <summary>
		/// Returns how long was ticket in the particular status
		/// </summary>
		/// <param name="status">The ticket status</param>
		public TimeSpan GetStatusDuration(TicketStatusEnum status, ITimestampService timestampService)
		{
			if (StatusHistory.Count > 0)
			{
				// calculate how long the ticket was in the desired status already in the past
				var history = StatusHistory.OrderBy(x => x.CreatedDate).ToList();
				var duration = TimeSpan.FromSeconds(history.Zip(history.Skip(1))
					.Where(x => x.First.Status == status)
					.Select(x => (x.Second.CreatedDate - x.First.CreatedDate))
					.Sum(x => x.TotalSeconds)
				);

				// if ticket is still in the desired status - account also that time
				var last = history.Last();
				if (last.Status == status)
				{
					duration += timestampService.GetUtcTimestamp() - last.CreatedDate;
				}

				return duration;
			}
			else
			{
				return new TimeSpan();
			}
		}

		private List<bool> GetTaggingStatuses()
		{
			List<bool> taggingStatuses = new List<bool>();

			if (Brand == null)
			{
				return taggingStatuses;
			}

			if (Brand.TagsEnabled)
			{
				CheckBrandTagStatuses(taggingStatuses);
			}

			if (Brand.CategoryEnabled && TransportType != TicketTransportType.Import)
			{
				taggingStatuses.Add(TicketCategory != null);
			}

			if (Brand.SposEnabled)
			{
				taggingStatuses.Add(SposLead != null);

				if(SposLead != null && SposLead.Value)
				{
					taggingStatuses.Add(!string.IsNullOrWhiteSpace(SposDetails));
				}
			}

			if (EscalatedById != null)
			{
				taggingStatuses.Add(CorrectlyDiagnosed != null);
			}

			if (IsValidTransferRequired)
			{
				taggingStatuses.Add(Level == TicketLevel.SuperSolver && ValidTransfer != null);
			}

			return taggingStatuses;
		}

		private void CheckBrandTagStatuses(List<bool> taggingStatuses)
		{
			if (Brand.Tags != null)
			{
				if (Brand.SubTagEnabled)
				{
					var parentTags = Brand.Tags.Where(x => x.ParentTagId == null).Select(x => x.Id).ToList();

					if (parentTags.Any())
					{
						var selectedParentTags = Tags.Where(x => parentTags.Contains(x.TagId)).Select(x => x.TagId).ToList();
						taggingStatuses.Add(selectedParentTags.Any());

						var childTags = Brand.Tags.Where(x => selectedParentTags.Contains(x.ParentTagId ?? Guid.Empty)).Select(x => x.Id).ToList();
						if (Level == TicketLevel.Regular)
						{
							childTags = Brand.Tags.Where(x => selectedParentTags.Contains(x.ParentTagId ?? Guid.Empty) && x.Level == null).Select(x => x.Id).ToList();
						}

						if (childTags.Any())
						{
							var selectedChildTags = Tags.Where(x => childTags.Contains(x.TagId)).ToList();
							taggingStatuses.Add(selectedChildTags.Any());
						}
					}
				}
				else
				{
					taggingStatuses.Add(Tags.Any());
				}
			}
			else
			{
				taggingStatuses.Add(false);
			}
		}
	}
}