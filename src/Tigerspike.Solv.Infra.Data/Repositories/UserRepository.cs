using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class UserRepository : Repository<User>, IUserRepository
	{
		public UserRepository(SolvDbContext dbContext) : base(dbContext) { }

		public async Task<bool?> IsUserEnabled(Guid userId)
		{
			return await GetFirstOrDefaultAsync(
				selector: x => (bool?)x.Enabled,
				predicate: x => x.Id == userId
			);
		}

		public Task<User> GetByEmail(string email) => GetFirstOrDefaultAsync(predicate: x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase) || x.AlternateEmail.Equals(email, StringComparison.InvariantCultureIgnoreCase), disableTracking: false);
	}
}