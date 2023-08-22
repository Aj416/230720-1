using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class QuizRepository : Repository<Quiz>, IQuizRepository
	{
		public QuizRepository(SolvDbContext context) : base(context) { }
	}
}
