using System;
using ServiceStack.DataAnnotations;

namespace Tigerspike.Solv.Services.IdentityVerification.Domain
{
	public class IdentityCheck
	{
		[HashKey]
		public string CheckId { get; set; }

		public Guid AdvocateId { get; set; }

		public string ApplicantId { get; set; }

		public string ReportUrl { get; set; }

		public DateTime CreatedDate { get; set; }

		public DateTime ModifiedDate { get; set; }

		public bool? Success { get; set; }

		protected IdentityCheck() {}

		public IdentityCheck(string checkId, Guid advocateId, string applicantId, string reportUrl)
		{
			CheckId = checkId;
			AdvocateId = advocateId;
			ApplicantId = applicantId;
			ReportUrl = reportUrl;
			CreatedDate = DateTime.UtcNow;
			ModifiedDate = DateTime.UtcNow;
		}

		public void SetValue(bool success)
		{
			Success = success;
			ModifiedDate = DateTime.UtcNow;
		}
	}
}