using System.Collections.Generic;

namespace Tigerspike.Solv.Services.IdentityVerification.Refit.CreateCheck
{
	public class CreateCheckRequest
	{
		public string ApplicantId { get; set; }
		public IList<ReportType> ReportNames { get; set; }
	}
}