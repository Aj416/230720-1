using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Services.Fraud.Enum;

namespace Tigerspike.Solv.Services.Fraud.Domain
{
	public class TicketDetection
	{
		/// <summary>
		/// Gets or sets the ticket id.
		/// </summary>
		[HashKey]
		public string TicketId { get; set; }

		/// <summary>
		/// Gets or sets the detection id.
		/// </summary>
		[RangeKey]
		public string DetectionId { get; set; }

		/// <summary>
		/// Gets or sets the date the detection was created.
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Gets or sets the rules that applied to this detection.
		/// </summary>
		public List<string> Rules { get; set; } = new List<string>();

		/// <summary>
		/// Gets or sets the status of the ticket when the detection occured.
		/// </summary>
		public TicketStatus Status { get; set; }

		// /// <summary>
		// /// Gets or sets the result of the detection. - High, Low, Medium
		// /// </summary>
		// public RiskLevel RiskLevel { get; set; }

		public TicketDetection(Guid ticketId, int status, List<Guid> rules)
		{
			DetectionId = Guid.NewGuid().ToString();
			TicketId = ticketId.ToString();
			CreatedDate = DateTime.UtcNow;
			Status = (TicketStatus)status;
			Rules.AddRange(rules.Select(r => r.ToString()));
		}
	}
}