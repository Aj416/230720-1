using System;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Domain.Models
{
	public class AdvocateBrand : ICreatedDate, IModifiedDate
	{
		public Guid AdvocateId { get; set; }
		public Guid BrandId { get; set; }
		public bool Authorized { get; set; }
		public bool AgreementAccepted { get; set; }
		public bool ContractAccepted { get; set; }
		public bool Inducted { get; set; }
		public bool Enabled { get; private set; }

		/// <summary>
		/// The average Csat that this advocate has received on all tickets he solved for this brand.
		/// </summary>
		public decimal Csat { get; private set; }

		/// <inheritdoc/>
		public DateTime CreatedDate { get; set; }

		/// <inheritdoc/>
		public DateTime ModifiedDate { get; set; }

		public bool Blocked { get; private set; }
		public DateTime? AuthorizedDate { get; set; }
		public DateTime? InductedDate { get; set; }
		public DateTime? ContractAcceptedDate { get; set; }

		public Brand Brand { get; set; }
		public User User { get; set; }
		public bool GuidelineAgreed { get; private set; }

		// A constructor to please EF.
		private AdvocateBrand() { }

		public AdvocateBrand(Guid advocateId, Guid brandId, bool authorised = false)
		{
			AdvocateId = advocateId;
			BrandId = brandId;

			if (authorised)
			{
				Enabled = true;
				Inducted = true;
				Authorized = true;
				ContractAccepted = true;
			}
		}

		public void SetEnabled(bool value) => Enabled = value;

		public void SetGuideline(bool value) => GuidelineAgreed = value;

		/// <summary>
		/// The advocate has accepted the agreement for the selected brand.
		/// </summary>
		public void AcceptBrandAgreement()
		{
			AgreementAccepted = true;
		}

		/// <summary>
		/// The advocate has accepted the contract for the selected brand.
		/// </summary>
		public void AgreeToContract(bool isAutomaticAuthorization, DateTime contractAcceptedDate)
		{
			ContractAccepted = true;
			ContractAcceptedDate = contractAcceptedDate;
			if (isAutomaticAuthorization)
			{
				Authorize();
			}
		}

		/// <summary>
		/// The advocate has passed the induction for the selected brand.
		/// </summary>
		public void PassInduction(bool isAutomaticAuthorization, DateTime inductedDate)
		{
			Inducted = true;
			InductedDate = inductedDate;
			if (isAutomaticAuthorization)
			{
				Authorize();
				AuthorizedDate = inductedDate;
			}
		}

		/// <summary>
		/// Checks if the advocate brand is ready to be authorized automatically
		/// </summary>
		private void Authorize() => Enabled = Authorized = ContractAccepted && Inducted;

		/// <summary>
		/// /// Update the csat of the advocate for this brand (mainly because he closed a new ticket and it was csat rated)
		/// </summary>
		public void SetCsat(decimal csat)
		{
			Csat = csat;
		}

		public void SetBlocked(bool value)
		{
			Blocked = value;
			Enabled = false;
			Inducted = false;
			Authorized = false;
			ContractAccepted = false;
		}
	}
}
