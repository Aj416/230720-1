using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS24232 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "ProfileStatus",
				table: "Advocate",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.SqlFromFile(nameof(DCTXS24232));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "ProfileStatus",
				table: "Advocate");
		}
	}
}
