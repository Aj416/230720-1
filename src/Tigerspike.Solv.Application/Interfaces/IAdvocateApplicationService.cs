using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Admin;
using Tigerspike.Solv.Application.Models.Profile;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface IAdvocateApplicationService
	{
		/// <summary>
		/// Returns all "enabled" Questions
		/// </summary>
		/// <returns>ProfileQuestion DTO that holds user information</returns>
		Task<ProfileQuestionsModel> GetAllEnabledQuestions(bool? isEnabled = null);

		/// <summary>
		/// Process all submitted answers
		/// </summary>
		/// <param name="profileAnswer"></param>
		/// <returns>True if succeeded</returns>
		Task SubmitAnswers(ProfileAnswerModel profileAnswer);

		/// <summary>
		/// Ensure submitted advocate answers are valid
		/// </summary>
		/// <param name="profileAnswer"></param>
		/// <returns>List of errors</returns>
		Task<IList<string>> ValidateAnswers(ProfileAnswerModel profileAnswer);

		/// <summary>
		/// Sends an invitation email to the candidates from the details of their application.
		/// </summary>
		/// <param name="advocateApplicationIds">The list of the advocate applications</param>
		Task InviteAdvocateFromApplication(IEnumerable<Guid> advocateApplicationIds);

		/// <summary>
		/// Declines an advocate applications from the details of their application.
		/// </summary>
		/// <param name="advocateApplicationIds">The list of the advocate applications</param>
		Task DeclineAdvocateApplication(IEnumerable<Guid> advocateApplicationIds);

		/// <summary>
		/// Sends the advocate applicant a confirmation email to delete their data.
		/// </summary>
		/// <param name="email">The applicants email</param>
		/// <returns>Whether sending the email succeeded</returns>
		Task<bool> SendDeleteAdvocateApplication(string email);

		/// <summary>
		/// Deletes an advocates application.
		/// </summary>
		/// <param name="email">The email of the advocate applicant.</param>
		/// <param name="key">The key that is used to validate ownership of the advocate's applications</param>
		/// <returns>True if successful, false otherwise</returns>
		Task<bool> DeleteAdvocateApplication(string email, string key);

		/// <summary>
		/// Exports the advocate application from the database and sends it to the advocates email
		/// address as a JSON file.
		/// </summary>
		/// <param name="email">The applicants email</param>
		/// <returns>Whether sending the email succeeded</returns>
		Task<bool> ExportAdvocateApplication(string email);

		/// <summary>
		/// Create an application for the advocate who showed interest in joining
		/// </summary>
		/// <param name="application">The application information</param>
		/// <returns>Nothing</returns>
		Task<Guid?> Apply(AdvocateApplicationModel application);

		/// <summary>
		/// Get all advocate applications.
		/// </summary>
		/// <returns>List of advocate applications</returns>
		Task<List<AdvocateApplicationModel>> GetAdvocateApplications();

		/// <summary>
		/// Get an advocate application.
		/// </summary>
		/// <param name="id">AdvocateApplication Id</param>
		/// <returns>An advocate applications</returns>
		Task<AdminAdvocateApplicationModel> GetAdvocateApplication(Guid id);

		/// <summary>
		/// Validate that the Application Id provided can still be used on the profiling section for
		/// a new Application
		/// </summary>
		/// <param name="id">The GUID of the AdvocateApplication</param>
		/// <returns>True if successful, false otherwise</returns>
		Task<bool> Validate(Guid id);

		/// <summary>
		/// Check whether an email is already in use
		/// </summary>
		/// <param name="email">Email address to check.</param>
		/// <returns>Whether email is in use or not</returns>
		Task<bool> IsEmailInUse(string email);

		/// <summary>
		/// Get Profile Brands
		/// </summary>
		/// <returns>List of Profile Brands</returns>
		Task<IList<ProfileBrandModel>> GetProfileBrands();

		/// <summary>
		/// Returns Admin Advocate Application
		/// </summary>
		/// <returns>Admin Advocate Application</returns>
		Task<IPagedList<AdvocateApplicationModel>> GetAdminAdvocateApplications(AdvocateApplicationStatus status = AdvocateApplicationStatus.New, int pageIndex = 0,
			int pageSize = 25, AdminAdvocateApplicationStatusSortBy sortBy = AdminAdvocateApplicationStatusSortBy.CreatedDate, SortOrder sortOrder = SortOrder.Desc);

		/// <summary>
		/// Returns all applications from system in export-friendly format
		/// <param name="country">The country name.</param>
		/// </summary>
		Task<IList<AdminAdvocateApplicationModel>> GetAllForExport(string country = null, bool sanitize = true);

		/// <summary>
		/// Get list of all assignable brands for advocate applications
		/// </summary>
		/// <param name="advocateApplicationId">The advocate application id</param>
		Task<IEnumerable<BrandInviteModel>> GetBrandAssignments(Guid advocateApplicationId);

		/// <summary>
		/// Validate the profile answer.
		/// </summary>
		/// <param name="model">ApplicationAnswerModel that needs to be validated.</param>
		/// <returns></returns>
		Task<bool> ValidateProfile(ApplicationAnswerModel model);

		/// <summary>
		/// Submit profile answer for advocate application one by one.
		/// </summary>
		/// <param name="advocateApplicationId">AdvocateApplicationId identifier.</param>
		/// <param name="model">Model of type ApplicationAnswerModel.</param>
		/// <returns></returns>
		Task SubmitProfileAnswers(Guid advocateApplicationId, ApplicationAnswerModel model);

		/// <summary>
		/// Return all the answers given by advocate in profile section.
		/// </summary>
		/// <param name="advocateApplicationId">AdvocateApplication Id identifier.</param>
		/// <returns>All the answers given by advocate in profile section.</returns>
		Task<IEnumerable<AdvocateApplicationProfileModel>> GetProfileAnswers(Guid advocateApplicationId);

		Task<bool> UpdateAdvocateApplicationwithSuperSolver();

		/// <summary>
		/// Returns all applications from system for candidate export
		/// </summary>
		/// <param name="from">from date</param>
		/// <param name="to">to date</param>
		Task<string> GetAllForExport(DateTime? from, DateTime? to);

	}
}