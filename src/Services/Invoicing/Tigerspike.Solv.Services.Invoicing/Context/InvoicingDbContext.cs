using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Tigerspike.Solv.Core.Behaviours;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.EntityFramework;
using Tigerspike.Solv.Services.Invoicing.Configuration.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Context
{
	public class InvoicingDbContext : DbContext
	{
		private readonly List<IContextBehaviour> _contextBehaviors = new List<IContextBehaviour>();
		private readonly string _connectionString;

		public InvoicingDbContext(DbContextOptions<InvoicingDbContext> builderOptions) : base(builderOptions)
		{

		}

		public InvoicingDbContext(DbContextOptions<InvoicingDbContext> builderOptions,
			IEnumerable<IContextBehaviour> contextBehaviors,
			IOptions<DatabaseOptions> dbSettingsAccessor) : base(builderOptions)
		{
			_contextBehaviors = contextBehaviors.ToList();
			_connectionString = new DbSettingsHelper(dbSettingsAccessor.Value).GetConnectionString();
		}

		public InvoicingDbContext(DbContextOptions<InvoicingDbContext> builderOptions, string connectionString) : base(builderOptions) => _connectionString = connectionString;

		public override int SaveChanges() => SaveChangesAsync(CancellationToken.None).Result;

		public override async Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default)
		{
			var contextEntries = ChangeTracker.Entries().ToList();
			_contextBehaviors.ForEach(x => x.Apply(contextEntries)); // apply configurated behaviors to current context entries

			return await base.SaveChangesAsync(cancellationToken);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.ReplaceService<IMigrationsSqlGenerator, CustomMySqlMigrationSqlGenerator>();
			if (optionsBuilder.IsConfigured == false) // if we are in unit testing and using InMemoryDatabase - it is already configured
			{
				// define the database to use
				optionsBuilder.UseMySql(_connectionString, builder => builder.ServerVersion(new ServerVersion(new Version(5, 7, 0), ServerType.MySql)));
			}

			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfigurationsFromAssembly(typeof(BillingDetailsConfiguration).Assembly);
		}
	}
}
