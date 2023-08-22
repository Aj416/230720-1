using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Tigerspike.Solv.Core.Configuration;

namespace Tigerspike.Solv.Infra.Data.Context
{
	public class SolvContextFactory : IDesignTimeDbContextFactory<SolvDbContext>
	{
		public SolvDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<SolvDbContext>();
			var connectionString = new DbSettingsHelper().GetConnectionString();
			return new SolvDbContext(builder.Options, connectionString);
		}
	}
}