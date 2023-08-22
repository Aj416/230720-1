using System;
using System.Collections.Generic;
using System.Linq;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Search
{
	/// <summary>
	/// A model that represents an advocate in the search index.
	/// </summary>
	public class AdvocateSearchModel
	{
		public Guid Id { get; set; }

		public string FullName { get; set; }

		/// <summary>
		/// The CSAT of the advocate.
		/// The default value is the one stored in the Advocate.CSAT (which is the average for all brands),
		/// However, if a brand was selected, the search service will overwrite it with
		/// the brand specific value.
		/// </summary>
		public decimal Csat { get; set; }

		/// <summary>
		/// The status of the advocate (Verified, Unverified ..etc).
		/// </summary>
		public AdvocateStatus Status { get; set; }

		/// <summary>
		/// The status of the advocate as text
		/// </summary>
		public string StatusText => Status.ToString();

		/// <summary>
		/// Represent the status of this advocate for a specific brand.
		/// This will be filled only at the time of searching and if the results are filtered by a brand.
		/// It is not intended to be used in the index.
		/// </summary>
		public string BrandStatus { get; set; }

		/// <summary>
		/// Indicates that the user has setup the payment method.
		/// </summary>
		public bool PaymentMethodSetup { get; set; }

		/// <summary>
		/// Indicates that the advocate has verified his payment account email.
		/// </summary>
		public bool PaymentEmailVerified { get; set; }

		/// <summary>
		/// Indicates whether the advocate watched the video.
		/// </summary>
		public bool VideoWatched { get; set; }

		/// <summary>
		/// Indicates whether the advocate has completed practice.
		/// </summary>
		public bool PracticeComplete { get; set; }

		/// <summary>
		/// Identity verification status
		/// </summary>
		public IdentityVerificationStatus IdentityVerificationStatus { get; set; }

		/// <summary>
		/// The list of brands that this advocate is assigned to,
		/// along with the individual status of each brand.
		/// </summary>
		public List<AdvocateBrandSearchModel> Brands { get; set; }

		/// <summary>
		/// The list of quiz attempts by advocate,
		/// along with the numner of failed attempts for each quiz.
		/// </summary>
		public List<AdvocateQuizSearchModel> QuizAttempts { get; set; } = new List<AdvocateQuizSearchModel>();

		///<summary>
		///This is just a work-around for Elasticsearch to sort by name, currently the current settings although set perfectly, still doesn't work.
		///TODO: Just remove this property, when this question is answered https://discuss.elastic.co/t/case-insensitive-sort-doesnt-work/143192
		///</summary>
		public string NameSortToken => FullName.Replace(" ", "").ToLower();

		/// <summary>
		/// How many onboarding actions have been completed
		/// </summary>
		public int OnboardingItemsCompleted { get; set; }

		/// <summary>
		/// Brands that are yet to be authorised.
		/// </summary>
		public IEnumerable<string> UnAuthorisedBrandNames => Brands.Where(x => x.StatusText
		.Equals(AdvocateSearchStatus.Unauthorized.ToString(), StringComparison.InvariantCultureIgnoreCase) && !x.IsPractice && !x.BrandBlocked)
		.Select(r => r.BrandName);

		public IEnumerable<string> BlockBrandNames => Brands.Where(x => x.BrandBlocked && !x.IsPractice).Select(x => x.BrandName);


		/// <summary>
		/// Invited status for unauthorised brand.
		/// </summary>
		public IEnumerable<string> InvitedStatus => Brands.Where(x => x.StatusText
		.Equals(AdvocateSearchStatus.Unauthorized.ToString(), StringComparison.InvariantCultureIgnoreCase) && !x.IsPractice && !x.BrandBlocked)
		.Select(r => GetInvitedStatus(r));

		/// <summary>
		/// Gets final status as per different conditional check for each unauthorised brand.
		/// </summary>
		private string GetInvitedStatus(AdvocateBrandSearchModel model)
		{

			var attempts = QuizAttempts.Where(qa => qa.QuizId == model.QuizId).Select(qa => qa.FailedAttempt).FirstOrDefault();
			var actualStatus = model.InviteStatus + (attempts > 0 ? 1 : 0);
			var actualEnum = Enum.TryParse<InvitedStatus>((actualStatus + 1).ToString(), out var result) ? result : Domain.Enums.InvitedStatus.None;
			return actualEnum == Domain.Enums.InvitedStatus.QuizFailed ? $"{actualEnum.ToDisplay()} x {attempts}" : actualEnum.ToDisplay();

		}


	}

	public class AdvocateBrandSearchModel
	{
		public Guid BrandId { get; set; }

		public int InductionStatus { get; set; }

		public int ContractStatus { get; set; }

		public int QuizStatus { get; set; }

		public int InviteStatus
		{
			get
			{
				return ContractStatus + InductionStatus + QuizStatus;
			}
		}

		public Guid? QuizId { get; set; }
		public bool IsPractice { get; set; }

		public string BrandName { get; set; }

		public bool Authorized { get; set; }

		public bool Enabled { get; set; }

		public bool Blocked { get; set; }

		public decimal Csat { get; set; }

		public bool BrandBlocked { get; set; }

		public AdvocateSearchStatus Status
		{
			get
			{
				if (Blocked)
				{
					return AdvocateSearchStatus.Blocked;
				}
				else if (Authorized)
				{
					if (Enabled)
					{
						return AdvocateSearchStatus.Authorized;
					}
					else
					{
						return AdvocateSearchStatus.Idle;
					}
				}
				else
				{
					return AdvocateSearchStatus.Unauthorized;
				}
			}
		}
		public string StatusText => Status.ToString();
	}

	public class AdvocateQuizSearchModel
	{
		public Guid QuizId { get; set; }
		public int FailedAttempt { get; set; }
	}

	public class AdvocateBlockedSearchModel
	{
		public string BrandName { get; set; }

		public DateTime BlockedDate { get; set; }
		public string BlockedDateText => String.Format("{0:dd/MM/yyyy}", BlockedDate);
	}
}