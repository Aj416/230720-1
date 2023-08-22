using System;
using System.Collections.Generic;
using System.Linq;
using Tigerspike.Solv.Core.Exceptions;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models.Induction;

namespace Tigerspike.Solv.Domain.Models
{
	public class Advocate
	{
		/// <summary>
		/// The maximum number of tickets can be assigned to an advocate.
		/// </summary>
		public const int MAX_ASSIGNED_TICKETS = 10;

		public Guid Id { get; private set; }

		/// <summary>
		/// The user information associated with this advocate.
		/// </summary>
		public User User { get; private set; }

		/// <summary>
		/// Private constructor for EF only
		/// </summary>
		private Advocate()
		{
			Brands = new List<AdvocateBrand>();
			BlockHistory = new List<AdvocateBlockHistory>();
		}

		public Advocate(User user, string countryCode, string source, bool internalAgent, bool verified, bool super)
		{
			// The advocate id should be the same as its user id.
			Id = user.Id;
			User = user;
			CountryCode = countryCode;
			Source = source;
			InternalAgent = internalAgent;
			VideoWatched = verified;
			PracticeComplete = verified;
			Super = super;
			Brands = new List<AdvocateBrand>();
			BlockHistory = new List<AdvocateBlockHistory>();
			ProfileStatus = (super || internalAgent) ? AdvocateApplicationProfileStatus.Completed : AdvocateApplicationProfileStatus.NotStarted;
		}

		/// <summary>
		/// Determines whether the advocate is an internal agent.
		/// </summary>
		public bool InternalAgent { get; set; }

		/// <summary>
		/// Determines whether the advocate is a super-solver.
		/// </summary>
		public bool Super { get; set; }

		/// <summary>
		/// Indicates that the user has setup the payment method.
		/// </summary>
		public bool PaymentMethodSetup { get; private set; }

		/// <summary>
		/// Indicates that the advocate has verified his payment account email.
		/// </summary>
		public bool PaymentEmailVerified { get; private set; }

		/// <summary>
		/// The account identifier of the payment method.
		/// </summary>
		public string PaymentAccountId { get; private set; }

		/// <summary>
		/// The average Csat that this advocate has received on all tickets he solved.
		/// </summary>
		public decimal Csat { get; private set; }

		/// <summary>
		/// Indicates whether the user should see a new brand notification next time he opens ticket pool.
		/// </summary>
		public bool ShowBrandNotification { get; private set; }

		/// <summary>
		/// Indicates whether the advocate watched the video.
		/// </summary>
		public bool VideoWatched { get; private set; }

		/// <summary>
		/// Indicates whether the advocate is in practice mode.
		/// </summary>
		public bool Practicing { get; private set; }

		/// <summary>
		/// Indicates whether the advocate has completed practice.
		/// </summary>
		public bool PracticeComplete { get; private set; }

		/// <summary>
		/// Country code where the advocate is registered to work
		/// </summary>
		public string CountryCode { get; private set; }

		/// <summary>
		/// Where the advocate heard of Solv.
		/// </summary>
		public string Source { get; private set; }

		/// <summary>
		/// A list of brands linking details that this advocate is associated with.
		/// </summary>
		public List<AdvocateBrand> Brands { get; private set; }

		/// <summary>
		/// A list of blocked brands linking details that this advocate is associated with.
		/// </summary>
		public List<AdvocateBlockHistory> BlockHistory { get; set; }

		/// <summary>
		/// A list of quiz attempts per brand that advocate is associated with.
		/// </summary>
		public List<QuizAdvocateAttempt> QuizAttempts { get; set; } = new List<QuizAdvocateAttempt>();

		/// <summary>
		/// Gets or sets the list of answers for the section items.
		/// </summary>
		public List<AdvocateSectionItem> AdvocateSectionItems { get; private set; }

		/// <summary>
		/// Decides the status of the advocate based on certian flags.
		/// </summary>
		public AdvocateStatus Status => User.Enabled ? VideoWatched && PracticeComplete ? AdvocateStatus.Verified : AdvocateStatus.Unverified : AdvocateStatus.Blocked;

		/// <summary>
		/// ApplicantId in identity verification system
		/// </summary>
		public string IdentityApplicantId { get; set; }

		/// <summary>
		/// Last CheckId in identity verification system
		/// </summary>
		public string IdentityCheckId { get; set; }

		/// <summary>
		/// Last Check result from identity verification system
		/// </summary>
		public string IdentityCheckResultUrl { get; set; }

		/// <summary>
		/// Identity verification status
		/// </summary>
		public IdentityVerificationStatus IdentityVerificationStatus { get; set; }

		/// <summary>
		/// Determines the profile application status of an advocate.
		/// </summary>
		public AdvocateApplicationProfileStatus ProfileStatus { get; set; }

		/// <summary>
		/// Set the video as watched.
		/// </summary>
		public void SetVideoWatched() => VideoWatched = true;

		/// <summary>
		/// Set identity applicant id
		/// </summary>
		public void SetIdentityApplicant(string identityApplicantId) => IdentityApplicantId = identityApplicantId;

		/// <summary>
		/// Set identity verification status
		/// </summary>
		public void SetIdentityVerificationStatus(IdentityVerificationStatus identityVerificationStatus) => IdentityVerificationStatus = identityVerificationStatus;

		/// <summary>
		/// Set identity check details
		/// </summary>
		public void SetIdentityCheckDetails(string identityCheckId, string identityCheckResultUrl)
		{
			IdentityCheckId = identityCheckId;
			IdentityCheckResultUrl = identityCheckResultUrl;
		}

		/// <summary>
		/// Set the payment account id.
		/// </summary>
		public void SetPaymentAccount(string accountId, bool emailVerified)
		{
			PaymentAccountId = accountId;
			PaymentEmailVerified = emailVerified;
			PaymentMethodSetup = !string.IsNullOrEmpty(accountId);
		}

		/// <summary>
		/// Starts the practice mode and adds the practice brand to the list.
		/// </summary>
		public void StartPractise(bool NewProfileEnable)
		{
			if (NewProfileEnable)
			{
				if (Practicing || PracticeComplete)
				{
					throw new DomainException("Advocate cannot start practicing, it is either already done/in-progress or advocate is missing previous steps (watch video)");
				}
			}
			else
			{
				if (Practicing || PracticeComplete || !VideoWatched)
				{
					throw new DomainException("Advocate cannot start practicing, it is either already done/in-progress or advocate is missing previous steps (watch video)");
				}
			}

			Practicing = true;

			// Add the practice brand for the user
			Brands.Add(new AdvocateBrand(Id, Brand.PracticeBrandId, true));
		}

		/// <summary>
		/// Ends the practice mode and remove the practice brand from the list.
		/// </summary>
		public void FinishPractice()
		{
			if (!Practicing || PracticeComplete)
			{
				throw new DomainException("The advocate is not practicing or already finished practicing.");
			}

			Practicing = false;
			PracticeComplete = true;
			// Remove the practice brand from advocate brands list.
			Brands = Brands.Where(b => b.BrandId != Brand.PracticeBrandId).ToList();
		}

		/// <summary>
		/// Set the brand notification value
		/// </summary>
		/// <param name="showBrandNotification">Whether to show or not the brand notification</param>
		public void SetBrandNotification(bool value) => ShowBrandNotification = value;

		/// <summary>
		/// Set the brand notification based on the specified brand
		/// </summary>
		/// <param name="advocateBrand">Particular advocate-brand connection</param>
		public void SetBrandNotification(AdvocateBrand advocateBrand) => ShowBrandNotification = advocateBrand.Authorized == false;

		public (IList<Guid> blockedBrands, IList<Guid> addedBrands, IList<Guid> unblockedBrands) SetBrands(IEnumerable<Guid> brandIds, bool authorised)
		{
			var toBlocked = Brands.Where(x => !x.Blocked).Select(x => x.BrandId).Except(brandIds).ToList();
			var toAdd = brandIds.Except(Brands.Select(x => x.BrandId)).ToList();
			var toUnblocked = Brands.Where(x => x.Blocked == true).Select(x => x.BrandId)
				.Except(Brands.Select(x => x.BrandId).Except(brandIds).ToList()).ToList();

			// Add brands
			Brands.AddRange(toAdd.Select(x => new AdvocateBrand(Id, x, authorised)));

			// removed induction history
			AdvocateSectionItems = AdvocateSectionItems?.Where(x => toBlocked.Contains(x.SectionItem.Section.BrandId) == false).ToList();

			return (toBlocked, toAdd, toUnblocked);
		}

		/// <summary>
		/// Add Advocate blocked history
		/// </summary>
		public void SetBlockedHistory(IList<Guid> blockedBrands, DateTime createdDate) =>
			BlockHistory.AddRange(blockedBrands.Select(x => new AdvocateBlockHistory(x, Id, createdDate)));

		/// <summary>
		/// Update the csat of the advocate (mainly because he closed a new ticket and it was csat rated)
		/// </summary>
		public void SetCsat(decimal csat) => Csat = csat;

		/// <summary>
		/// Gets CSAT for the specified Brand (if applicable)
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <returns>CSAT for the specified brand, or general CSAT if not available</returns>
		public decimal GetCsat(Guid brandId) => Brands?
			.Where(x => x.BrandId == brandId)
			.Select(x => (decimal?)x.Csat)
			.SingleOrDefault() ?? Csat;
	}
}