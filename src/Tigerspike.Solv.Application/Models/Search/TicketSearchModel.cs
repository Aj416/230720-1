using System;
using System.Collections.Generic;
using System.Linq;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Application.Models.Search
{
	public class TicketSearchModel : TicketModel
	{
		public const string DateFormat = "dd MMM yy, hh:mm:ss";

		/// <summary>
		/// Number of minutes from the last advocate reply before it is marked as Awaiting.
		/// </summary>
		public const int AwaitingPeriodMinutes = 60;

		public string CreatedDateText => CreatedDate.ToString(DateFormat);

		public DateTime? ClosedDate { get; set; }

		public string ClosedDateText => ClosedDate?.ToString(DateFormat);

		public DateTime? AssignedDate { get; set; }

		public string AssignedDateText => AssignedDate?.ToString(DateFormat);

		public DateTime? FirstAssignedDate { get; set; }

		public string FirstAssignedDateText => FirstAssignedDate?.ToString(DateFormat);

		/// <summary>
		/// The date of the last message received from the customer (if any)
		/// </summary>
		public DateTime? LastCustomerMessageDate { get; set; }

		/// <summary>
		/// The date of the last message received from the advocate (if any)
		/// </summary>
		public DateTime? LastAdvocateMessageDate { get; set; }

		public int? AbsoluteTimeToClose { get; set; }

		public int? SolverTimeToClose { get; set; }

		public bool? IsAbandoned { get; set; }

		public bool IsCritical { get; set; }

		public string CurrentAdvocateFullName { get; set; }

		public string LastAdvocateFullName => AdvocatesHistory?.LastOrDefault();

		public string AdvocateFullName => Status != TicketStatusEnum.Escalated ? CurrentAdvocateFullName : LastAdvocateFullName;

		public IEnumerable<string> AdvocatesHistory { get; set; }

		public string BrandName { get; set; }

		public string BrandNameSortToken => BrandName?.Replace(" ", "").ToLower();

		/// <summary>
		/// Used for the search to help indexing
		/// </summary>
		public string AdvocateFullNameSortToken => AdvocateFullName?.Replace(" ", "").ToLower();

		public double TimeOpen => AbsoluteTimeToClose ?? (DateTime.UtcNow - CreatedDate).TotalSeconds;

		public Guid? AdvocateId { get; set; }

		/// <summary>
		/// The status of the ticket as text
		/// </summary>
		public string StatusText => Status.ToString();

		/// <summary>
		/// The last escalation date of the ticket.
		/// </summary>
		public DateTime? EscalatedDate { get; set; }

		public string EscalatedDateText => EscalatedDate?.ToString(DateFormat);

		/// <summary>
		/// The Fraud status of the ticket as text
		/// </summary>
		public string FraudStatusText => FraudStatus.ToString();

		public IEnumerable<TicketAbandonHistoryModel> AbandonHistory { get; set; }
		public string LastAbandonmentReasonsNames => AbandonHistory?.Count() > 0
			? AbandonHistory
				.OrderBy(x => x.CreatedDate)
				.LastOrDefault()
				.Reasons.Concatenate("|", null)
			: null;

		public string ProbingAnwersEscalationReasons => ProbingAnswers?
			.Where(x => x.Action != null)
			.Select(x => x.QuestionCode)
			.Concatenate("|", null);


		/// <summary>
		/// The reason of the escalation (text)
		/// </summary>
		public string EscalationReasonText
		{
			get
			{
				switch (EscalationReason)
				{
					case TicketEscalationReason.OpenTimeExceeded:
						return "Timed out";
					case TicketEscalationReason.AbandonedCountExceeded:
						return "Abandoned too many times";
					case TicketEscalationReason.RejectionCountExceeded:
						return "Rejected too many times";
					case TicketEscalationReason.AdminEscalated:
						return "Admin escalated";
					case TicketEscalationReason.Tag:
						return "Solver escalated";
					case TicketEscalationReason.ProbingForm:
						return ProbingAnwersEscalationReasons;
					case TicketEscalationReason.AbandonReason:
						return LastAbandonmentReasonsNames;
					case TicketEscalationReason.Urgent:
						return "Urgent";
					default:
						return null;
				}
			}
		}

		public string EscalationReasonSortToken => EscalationReasonText?.Replace(" ", "").ToLower();

	}
}