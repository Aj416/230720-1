using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models.Profile;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class AnswerRepository : Repository<Answer>, IAnswerRepository
	{
		public AnswerRepository(SolvDbContext context) : base(context) { }
	}
}