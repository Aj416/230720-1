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
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.EntityFramework;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Models.Induction;
using Tigerspike.Solv.Domain.Models.Profile;
using Tigerspike.Solv.Infra.Data.Configuration;
using Tigerspike.Solv.Infra.Data.Context.Behaviors;

namespace Tigerspike.Solv.Infra.Data.Context
{
	public class SolvDbContext : DbContext
	{
		private readonly List<IContextBehavior> _contextBehaviors = new List<IContextBehavior>();
		private readonly string _connectionString;

		public SolvDbContext(DbContextOptions<SolvDbContext> builderOptions) : base(builderOptions)
		{

		}

		public SolvDbContext(DbContextOptions<SolvDbContext> builderOptions,
			IEnumerable<IContextBehavior> contextBehaviors,
			IOptions<DatabaseOptions> dbSettingsAccessor) : base(builderOptions)
		{
			_contextBehaviors = contextBehaviors.ToList();
			_connectionString = new DbSettingsHelper(dbSettingsAccessor.Value).GetConnectionString();
		}

		public SolvDbContext(DbContextOptions<SolvDbContext> builderOptions, string connectionString) : base(builderOptions) => _connectionString = connectionString;

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

			modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketConfiguration).Assembly);
		}

		#region DbSets - TODO - we do not use those public sets from DbContext in the application in general so we could probably get rid of them from here altogether

		public virtual DbSet<Question> Questions { get; set; }
		public virtual DbSet<Section> InductionSections { get; set; }
		public virtual DbSet<SectionItem> InductionSectionItems { get; set; }
		public virtual DbSet<AdvocateSectionItem> InductionAdvocateSectionItems { get; set; }

		#endregion
	}
}