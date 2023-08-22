using System.Collections.Generic;
using Tigerspike.Solv.Infra.Data.Models;

namespace Tigerspike.Solv.Application.Models.Brand
{
	public class BrandPublicModel
	{
		public string Name { get; set; }
		public string Code { get; set; }
		public string ShortCode { get; set; }
		public string Logo { get; set; }
		public bool NpsEnabled { get; set; }
		public string CreateTicketHeader { get; set; }
		public string CreateTicketSubheader { get; set; }
		public string CreateTicketInstructions { get; set; }
		public string AdvocateTitle { get; set; }
		public List<BrandFieldModel> AdditionalFields { get; set; }
		public ProbingFormModel ProbingForm { get; set; }
		public bool AdditionalFeedBackEnabled { get; set; }
		public bool EndChatEnabled { get; set; }
		public string RedirectUrl { get; set; }
		public bool SkipCustomerForm { get; private set; }
		public string ProbingFormRedirectUrl { get; private set; }

	}
}
