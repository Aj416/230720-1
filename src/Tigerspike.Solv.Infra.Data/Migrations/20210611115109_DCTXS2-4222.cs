using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS24222 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "OptionsPerRow",
				table: "ProfileQuestion",
				nullable: true);

			migrationBuilder.SqlFromFile(nameof(DCTXS24222));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "OptionsPerRow",
				table: "ProfileQuestion");
		}
	}
}
