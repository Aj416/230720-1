using System;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Commands.Validations;

namespace Tigerspike.Solv.Domain.Commands.Brand
{
	public class CreateBrandCommand : Command<Guid?, CreateBrandCommandValidator>
	{
		public string Name { get; set; }
		public string Code { get; set; }
		public string ShortCode { get; set; }
		public string Thumbnail { get; set; }
		public string Logo { get; set; }

		/// <summary>
		/// The url of the text of the contract of this brand (for normal solvers, not internal agents)
		/// </summary>
		public string ContractUrl { get; set; }

		/// <summary>
		/// The url of the text of the contract of this brand (for internal agents)
		/// </summary>
		public string ContractInternalUrl { get; set; }
		public string CreateTicketInstructions { get; set; }
		public string CreateTicketHeader { get; set; }
		public string CreateTicketSubheader { get; set; }
		public string AdvocateTitle { get; set; }
		public decimal TicketPrice { get; set; }
		public decimal FeePercentage { get; set; }
		public decimal? VatRate { get; set; }
		public int? PaymentRouteId { get; set; }
		public bool AutomaticAuthorization { get; set; }
		public string InductionDoneMessage { get; set; }
		public string InductionInstructions { get; set; }
		public string UnauthorizedMessage { get; set; }
		public string AgreementContent { get; set; }
		public string AgreementHeading { get; set; }
		public bool IsAgreementRequired { get; set; }
		public bool InvoicingEnabled { get; set; }
		public bool InvoicingDashboardEnabled { get; set; }
		public bool TagsEnabled { get; set; }
		public bool NpsEnabled { get; set; }
		public string SposEmail { get; set; }
		public bool SposEnabled { get; set; }
		public bool TicketsExportEnabled { get; set; }
		public bool SuperSolversEnabled { get; set; }
		public bool PushBackToClientEnabled { get; set; }
		public int WaitMinutesToClose { get; set; }
		public string Color { get; set; }
		public bool TicketsImportEnabled { get; set; }
		public bool SubTagEnabled { get; set; }
		public bool MultiTagEnabled { get; set; }
		public bool CategoryEnabled { get; set; }
		public bool ValidTransferEnabled { get; set; }
		public bool AdditionalFeedBackEnabled { get; set; }
		public bool EndChatEnabled { get; set; }
		public string AutoRedirectUrl { get; set; }
	}
}