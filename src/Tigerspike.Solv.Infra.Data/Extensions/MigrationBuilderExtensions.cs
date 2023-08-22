using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace Tigerspike.Solv.Infra.Data
{
	public static class MigrationBuilderExtensions
	{
		public static OperationBuilder<SqlOperation> SqlFromFile(this MigrationBuilder builder, string fileName)
		{
			var path = Path.ChangeExtension(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migrations", fileName), ".sql");
			var script = File.ReadAllText(path);
			return builder.Sql(script);
		}
	}
}


