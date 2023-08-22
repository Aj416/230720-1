using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class QuizAdvocateAttemptRepository : Repository<QuizAdvocateAttempt>, IQuizAdvocateAttemptRepository
	{
		public QuizAdvocateAttemptRepository(SolvDbContext dbContext) : base(dbContext) { }

	}
}