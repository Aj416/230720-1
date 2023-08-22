using System;

namespace Tigerspike.Solv.Application.Models
{
	public class AdvocateBrandModel
	{
		// Association details
		public bool Authorized { get; set; }
		public bool AgreementAccepted { get; set; }
		public bool ContractAccepted { get; set; }
		public bool Inducted { get; set; }
		public bool Enabled { get; set; }
		public bool Blocked { get; set; }

		// Brand details
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }
		public string ShortCode { get; set; }
		public string Thumbnail { get; set; }
		public string Logo { get; set; }
		public bool IsPractice { get; set; }

		/// <summary>
		/// The url of the text of the contract of this brand (for normal solvers, not internal agents)
		/// </summary>
		public string ContractUrl { get; private set; }

		/// <summary>
		/// The url of the text of the contract of this brand (for internal agents)
		/// </summary>
		public string ContractInternalUrl { get; private set; }

		/// <summary>
		/// The message that is shown when the solver has done induction and signed the contract
		/// yet he is still needs to be authorized manually by the client.
		/// </summary>
		public string UnauthorizedMessage { get; private set; }

		/// <summary>
		/// The instructions to be shown when the solver has done the induction
		/// </summary>
		public string InductionDoneMessage { get; private set; }

		/// <summary>
		/// HTML content of the instructions
		/// </summary>
		public string InductionInstructions { get; set; }

		/// <summary>
		/// If true, the solver will be authorized to a brand automatically after doing the induction
		/// and signing the contract.
		/// </summary>
		public bool AutomaticAuthorization { get; set; }

		/// <summary>
		/// The title that is shown when the solver opens agreement modal.
		/// </summary>
		public string AgreementHeading { get; private set; }

		/// <summary>
		/// The content that is shown when the solver opens agreement modal.
		/// </summary>
		public string AgreementContent { get; private set; }

		/// <summary>
		/// If true, the solver must pass quiz for a brand to become authorized.
		/// </summary>
		public bool IsQuizRequired { get; set; }

		/// <summary>
		/// If true, the solver must accept agreement before proceesing with getting authorized for
		/// a brand
		/// </summary>
		public bool IsAgreementRequired { get; set; }

		/// <summary>
		/// Determines whether invoicing dashboard is enabled for the brand.
		/// If disabled, all widgets that shows prices, due amount, invoices .. are disabled on FE.
		/// </summary>
		public bool InvoicingDashboardEnabled { get; set; }

		/// <summary>
		/// Whether tags feature is enabled for this brand or not
		/// </summary>
		public bool TagsEnabled { get; set; }

		/// <summary>
		/// Whether NPS feature is enabled for this brand or not
		/// </summary>
		public bool NpsEnabled { get; set; }

		/// <summary>
		/// Whether SPOS feature is enabled for this brand or not
		/// </summary>
		public bool SposEnabled { get; set; }

		/// <summary>
		/// The color of the brand.
		/// </summary>
		public string Color { get; private set; }

		/// <summary>
		/// Determines if sub tags enabled for a brand.
		/// </summary>
		public bool SubTagEnabled { get; set; }

		/// <summary>
		/// Determines if multiselection of tags/sub tags enabled.
		/// </summary>
		public bool MultiTagEnabled { get; set; }

		/// <summary>
		/// Determines if category is enabled.
		/// </summary>
		public bool CategoryEnabled { get; set; }

		/// <summary>
		/// Description for SPOS
		/// </summary>
		public string SposDescription { get; private set; }

		/// <summary>
		/// Description for Category
		/// </summary>
		public string CategoryDescription { get; private set; }

		/// <summary>
		/// Description for ValidTransfer
		/// </summary>
		public string ValidTransferDescription { get; private set; }

		/// <summary>
		/// Description for Diagnosis
		/// </summary>
		public string DiagnosisDescription { get; private set; }
	}
}