using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Infra.Data.Models.Cached
{
	public class Brand
	{
		public Guid Id { get; set; }

		public string Name { get; set; }
		public string Code { get; set; }
		public string ShortCode { get; set; }

		public string Thumbnail { get; set; }

		public string Logo { get; set; }

		public string ContractUrl { get; set; }

		public string ContractInternalUrl { get; set; }

		public bool IsPractice { get; set; }

		public decimal FeePercentage { get; set; }

		public decimal TicketPrice { get; set; }

		public Guid BillingDetailsId { get; set; }

		public string PaymentAccountId { get; set; }

		public decimal? VatRate { get; set; }

		public string UnauthorizedMessage { get; set; }

		public bool AutomaticAuthorization { get; set; }

		public string AgreementHeading { get; set; }

		public string AgreementContent { get; set; }

		public bool IsAgreementRequired { get; set; }

		public Guid? QuizId { get; set; }

		public string InductionDoneMessage { get; set; }

		public string InductionInstructions { get; set; }

		public int? PaymentRouteId { get; set; }

		public bool InvoicingEnabled { get; set; }

		public bool InvoicingDashboardEnabled { get; set; }

		public string CreateTicketHeader { get; set; }
		public string CreateTicketSubheader { get; set; }

		public string CreateTicketInstructions { get; set; }

		public string AdvocateTitle { get; set; }

		public bool TagsEnabled { get; set; }

		public bool NpsEnabled { get; set; }
		public bool SposEnabled { get; set; }

		public bool TicketsExportEnabled { get; set; }

		public bool SuperSolversEnabled { get; set; }

		public bool PushBackToClientEnabled { get; set; }

		public int WaitMinutesToClose { get; set; }

		public string Color { get; set; }

		public DateTime CreatedDate { get; set; }

		public List<BrandFormField> FormFields { get; set; }

		public ProbingFormModel ProbingForm { get; set; }

		public bool AdditionalFeedBackEnabled { get; set; }

		public bool EndChatEnabled { get; set; }
		public string AutoRedirectUrl { get; set; }
		public bool SkipCustomerForm { get; set; }
		public string ProbingFormRedirectUrl { get; set; }
	}
}