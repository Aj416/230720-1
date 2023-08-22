using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Migrations;

namespace Tigerspike.Solv.Core.EntityFramework
{
	/// <summary>
	/// Custom MigrationSqlGenerator to add semi-colons to the end of
	/// all migration statements.
	/// </summary>
	public class CustomMySqlMigrationSqlGenerator : MySqlMigrationsSqlGenerator
	{
		private readonly MigrationsSqlGeneratorDependencies _dependencies;

#pragma warning disable EF1001
		public CustomMySqlMigrationSqlGenerator(MigrationsSqlGeneratorDependencies dependencies,
			IMigrationsAnnotationProvider annotationProvider, IMySqlOptions options) : base(dependencies,
			annotationProvider, options)
		{
			_dependencies = dependencies;
		}
#pragma warning restore EF1001


		public override IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations, IModel model = null)
		{
			var result = new List<MigrationCommand>();
			IEnumerable<MigrationCommand> statements = base.Generate(operations, model);
			foreach (var statement in statements)
			{
				var factory = _dependencies.CommandBuilderFactory.Create();
				factory.Append((statement.CommandText.TrimEnd() + ";").Replace(";;", ";"));
				result.Add(new MigrationCommand(factory.Build(), null, _dependencies.Logger, statement.TransactionSuppressed));
			}
			return result;
		}
	}
}