using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class AdvocateApplicationRepository : Repository<AdvocateApplication>, IAdvocateApplicationRepository
	{
		public AdvocateApplicationRepository(SolvDbContext context) : base(context) { }

		/// <inheritdoc/>
		public async Task<IList<AdvocateApplication>> GetFullAdvocateApplication(Expression<Func<AdvocateApplication, bool>> condition = null)
		{
			var query = Queryable();
			if (condition != null)
			{
				query = query.Where(condition);
			}

			var applications = await query.ToListAsync();

			await query
				.Include(y => y.ApplicationAnswers)
				.ThenInclude(y => y.Answers)
				.ThenInclude(y => y.QuestionOption)
				.LoadAsync();

			await query
				.Include(y => y.ApplicationAnswers)
				.ThenInclude(y => y.Question)
				.ThenInclude(y => y.QuestionType)
				.Include(y => y.ApplicationAnswers)
				.ThenInclude(y => y.Question)
				.ThenInclude(y => y.QuestionOptions)
				.LoadAsync();

			await query
				.Include(y => y.BrandAssignments)
				.ThenInclude(y => y.Brand)
				.LoadAsync();

			return applications;
		}

		/// <inheritdoc/>
		public async Task<IList<AdvocateApplication>> GetExportData(DateTime? from, DateTime? to)
		{
			var fromTimestamp = from ?? DateTime.MinValue;
			var toTimestamp = to ?? DateTime.MaxValue;
			var query = Queryable()
				.Where(x => x.ApplicationStatus != AdvocateApplicationStatus.AccountCreated)
				.Where(x => (x.CreatedDate.Date >= fromTimestamp && x.CreatedDate.Date <= toTimestamp));

			var applications = await query.ToListAsync();

			await query
				.Include(y => y.BrandAssignments)
				.ThenInclude(y => y.Brand)
				.LoadAsync();

			return applications;
		}

		/// <inheritdoc/>
		public async Task<bool> AreApplicationsNotInvited(Guid[] advocateApplicationIds)
		{
			var query = from app in Queryable()
						where advocateApplicationIds.Contains(app.Id) &&
							 app.ApplicationStatus != AdvocateApplicationStatus.Invited
						select app.Id;
			var count = await query.CountAsync();
			return count == advocateApplicationIds.Count();
		}

		/// <inheritdoc/>
		public async Task<bool> IsEmailInUse(string email) => await Queryable().AnyAsync(x => x.Email == email);

		/// <inheritdoc/>
		public async Task<IEnumerable<Guid>> GetAssignedBrands(Guid advocateApplicationId)
		{
			return await Queryable()
				.Where(x => x.Id == advocateApplicationId)
				.SelectMany(x => x.BrandAssignments)
				.Select(x => x.BrandId)
				.ToListAsync();
		}

	}
}