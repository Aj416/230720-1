using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS24338 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "Enabled",
				table: "ProfileQuestionOptionCombo",
				nullable: false,
				defaultValue: true);

			migrationBuilder.AddColumn<int>(
				name: "OptionsPerRow",
				table: "ProfileQuestionOptionCombo",
				nullable: false,
				defaultValue: 0);

            migrationBuilder.SqlFromFile(nameof(DCTXS24338));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Enabled",
				table: "ProfileQuestionOptionCombo");

			migrationBuilder.DropColumn(
				name: "OptionsPerRow",
				table: "ProfileQuestionOptionCombo");
		}
	}
}
