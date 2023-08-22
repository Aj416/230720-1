using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS24172 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Header",
				table: "ProfileQuestion",
				nullable: true);

			migrationBuilder.AddColumn<bool>(
				name: "Enabled",
				table: "ProfileArea",
				nullable: false,
				defaultValue: false);

			migrationBuilder.SqlFromFile(nameof(DCTXS24172));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Header",
				table: "ProfileQuestion");

			migrationBuilder.DropColumn(
				name: "Enabled",
				table: "ProfileArea");
		}
	}
}
