using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Services.Invoicing.Context;
using Tigerspike.Solv.Services.Invoicing.Domain;
using Tigerspike.Solv.Services.Invoicing.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.Invoicing.Infrastructure.Repositories
{
	public class SequenceRepository : Repository<Sequence>, ISequenceRepository
	{
		public SequenceRepository(InvoicingDbContext context) : base(context)
		{
		}

		/// </inheritdoc/>
		public async Task<int> Next(string sequenceName)
		{
			// UPSERT equivalent for MySQL - we need to realize one atomic instruction for reserving the next number of the sequence
			await DbContext.Database.ExecuteSqlRawAsync(
				$"INSERT {nameof(Sequence)} VALUES(@p0, 1) ON DUPLICATE KEY UPDATE {nameof(Sequence.Value)} = {nameof(Sequence.Value)} + 1",
				sequenceName);
			var res = await DbSet.FindAsync(sequenceName);
			return res.Value;
		}
	}
}
