using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface IAdvocateApplicationRepository : IRepository<AdvocateApplication>
	{
		/// <summary>
		/// Get Full advocate applications
		/// </summary>
		/// <param name="condition">Get Full advocate applications</param>
		Task<IList<AdvocateApplication>> GetFullAdvocateApplication(Expression<Func<AdvocateApplication, bool>> condition = null);

		/// <summary>
		/// Return specified email is already used in some application
		/// </summary>
		/// <param name="email">Email</param>
		/// <returns>True if email is already used, false otherwise</returns>
		Task<bool> IsEmailInUse(string email);

		/// <summary>
		/// Validate if the applications exists and are not invited.
		/// </summary>
		/// <param name="advocateApplicationIds">The list of ids to be validated</param>
		Task<bool> AreApplicationsNotInvited(Guid[] advocateApplicationIds);

		/// <summary>
		/// Get brands assigned to advocate application
		/// </summary>
		/// <param name="advocateApplicationId">The advocate application id</param>
		Task<IEnumerable<Guid>> GetAssignedBrands(Guid advocateApplicationId);

		/// <summary>
		/// Get Export advocate applications
		/// </summary>
		/// <param name="from">From date</param>
		/// <param name="to">To date</param>
		Task<IList<AdvocateApplication>> GetExportData(DateTime? from, DateTime? to);
	}
}