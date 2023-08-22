using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS23960 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder) =>
			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23960));

		protected override void Down(MigrationBuilder migrationBuilder)
		{
		}
	}
}
