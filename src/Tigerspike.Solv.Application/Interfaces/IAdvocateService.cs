using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Induction;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Application.Models.Statistics;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface IAdvocateService
	{
		/// <summary>
		/// Creates the advocate.
		/// </summary>
		Task Create(string token, string password);

		/// <summary>
		/// Find the advocate that has the passed id
		/// </summary>
		/// <param name="advocateId">The user id of the advocate</param>
		/// <param name="brandId">The brand id that the advocate should be linked to (if null, it is ignored)</param>
		/// <returns>Advocate info</returns>
		Task<AdvocateModel> FindAsync(Guid advocateId, Guid? brandId = null);

		/// <summary>
		/// Checks if the advocate exists.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <param name="brandId">An optional brand id.</param>
		/// <returns>True if the advocate exists, false otherwise.</returns>
		Task<bool> ExistsAsync(Guid advocateId, Guid? brandId = null);

		/// <summary>
		/// Sets the video as watched for the advocate.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		Task SetVideoWatched(Guid advocateId);

		/// <summary>
		/// Check the application token if it is valid
		/// </summary>
		/// <param name="token">The application token</param>
		/// <returns>True if valid</returns>
		Task<bool> ValidateToken(string token);

		/// <summary>
		/// Start the practice mode of an advocate.
		/// </summary>
		/// <param name="advocateId"></param>
		Task StartPractice(Guid advocateId);


		/// <summary>
		/// Mark that an advocate has accepted brand agreement.
		/// </summary>
		/// <param name="advocateId">Advocate ID</param>
		/// <param name="brandId">Brand ID</param>
		Task AcceptBrandAgreement(Guid advocateId, Guid brandId);

		/// <summary>
		/// Mark that an advocate has agreed to brand's contract.
		/// </summary>
		/// <param name="advocateId">Advocate ID</param>
		/// <param name="brandId">Brand ID</param>
		Task AgreeToContract(Guid advocateId, Guid brandId);

		/// <summary>
		/// Return true if the status of the advocate is Verified.
		/// </summary>
		Task<bool> IsVerified(Guid advocateId);

		/// <summary>
		/// Mark that an advocate has enabled the specified brand.
		/// </summary>
		/// <param name="advocateId">Advocate ID</param>
		/// <param name="brandId">Brand ID</param>
		Task EnableBrand(Guid advocateId, Guid brandId);

		/// <summary>
		/// Mark that an advocate has disabled the specified brand.
		/// </summary>
		/// <param name="advocateId">Advocate ID</param>
		/// <param name="brandId">Brand ID</param>
		Task DisableBrand(Guid advocateId, Guid brandId);

		/// <summary>
		/// Gets the advocate status statistics.
		/// </summary>
		/// <returns>Count of advocates in certain status</returns>
		Task<AdvocateStatisticByStatusModel> GetStatisticsByStatusForAll();

		/// <summary>
		/// Gets the advocate status statistics for the specified brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <returns>Count of advocates in certain status</returns>
		Task<AdvocateStatisticByStatusModel> GetStatisticsByStatusForBrand(Guid brandId);

		/// <summary>
		/// Get the advocate information formatted for search index.
		/// </summary>
		Task<AdvocateSearchModel> GetAdvocateForSearch(Guid advocateId);

		/// <summary>
		/// Gets the induction model of the advocate for the specified brand.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <param name="brandId">The brand Id.</param>
		/// <returns>The induction status of the advocate for the brand.</returns>
		Task<AdvocateInductionModel> GetInduction(Guid advocateId, Guid brandId);

		/// <summary>
		/// Sets the induction item to viewed by the advocate.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <param name="brandId">The brand id.</param>
		/// <param name="itemId">The item id.</param>
		Task MarkInductionItem(Guid advocateId, Guid brandId, Guid itemId);

		/// <summary>
		/// Sets Guideline Agreed by the advocate.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <param name="brandId">The brand id.</param>
		Task SetGuideLine(Guid advocateId, Guid brandId);

		/// <summary>
		/// Marks quiz for the brand as passed for the advocate
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		/// <param name="brandId">The brand id</param>
		Task MarkQuizAsPassed(Guid advocateId, Guid brandId);

		/// <summary>
		///	Trigger the call to payment gateway servers (PayPal currently)
		/// To get the latest status of the advocate if changed ever.
		/// </summary>
		/// <param name="advocateId">The advocate id to check</param>
		Task<(bool paymentMethodSetup, bool emailVerified, string paymentAccountId)> UpdatePaymentMethodStatus(Guid advocateId);

		/// <summary>
		/// Set brands assigned to the advocate
		/// </summary>
		/// <param name="advocateId">The advocate to whom we assign brands</param>
		/// <param name="brandIds">The ids of brands to be listed</param>
		/// <param name="authorised">Whether the advocate should be automatically authorised for the brand</param>
		/// <returns></returns>
		Task SetBrands(Guid advocateId, IEnumerable<Guid> brandIds, bool authorised);

		/// <summary>
		/// Create new SuperSolver account
		/// </summary>
		/// <param name="firstName">User first name</param>
		/// <param name="lastName">User last name</param>
		/// <param name="email">User password</param>
		/// <param name="countryCode">User country code (not required)</param>
		/// <param name="phone">User phone (not required)</param>
		/// <param name="password">User password</param>
		Task<Guid?> CreateSuperSolver(string firstName, string lastName, string email, string countryCode, string phone, string password);

		/// <summary>
		/// Returns Admin Advocate Application
		/// </summary>
		/// <returns>Admin Advocate Application</returns>
		Task<string> GetAllAdvocate();
	}
}