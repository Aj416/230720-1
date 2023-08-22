using System;

namespace Tigerspike.Solv.Application.Models
{
	public class BrandModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }
		public string ShortCode { get; set; }
		public string Thumbnail { get; set; }
		public string Logo { get; set; }
		public bool NpsEnabled { get; set; }
		public bool SposEnabled { get; set; }
		public bool TicketsExportEnabled { get; set; }
		public bool IsPractice { get; set; }
		public decimal TicketPrice { get; set; }
		public decimal FeePercentage { get; set; }
		public bool HasEscalationFlow { get; set; }
		public bool InvoicingDashboardEnabled { get; set; }
		public bool SuperSolversEnabled { get; set; }
		public bool PushBackToClientEnabled { get; set; }
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The url of the text of the contract of this brand (for normal solvers, not internal agents)
		/// </summary>
		public string ContractUrl { get; private set; }

		/// <summary>
		/// The url of the text of the contract of this brand (for internal agents)
		/// </summary>
		public string ContractInternalUrl { get; private set; }

		/// <summary>
		/// The color of the brand.
		/// </summary>
		public string Color { get; private set; }

		/// <summary>
		/// Determines if Tickets import is enabled for the brand.
		/// </summary>
		public bool TicketsImportEnabled { get; set; }

		/// <summary>
		/// Whether tags feature is enabled for this brand or not
		/// </summary>
		public bool TagsEnabled { get; set; }

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

		/// <summary>
		/// Determines if additional feedback enabled for a brand.
		/// </summary>
		public bool AdditionalFeedBackEnabled { get; private set; }

		/// <summary>
		/// Determines if end chat is enabled for the brand.
		/// </summary>
		public bool EndChatEnabled { get; private set; }
	}
}
