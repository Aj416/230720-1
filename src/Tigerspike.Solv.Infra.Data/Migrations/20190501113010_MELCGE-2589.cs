using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class MELCGE2589 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "AdvocateApplication",
				columns : table => new
				{
					AdvocateApplicationId = table.Column<Guid>(nullable: false),
						CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
						Country = table.Column<string>(maxLength: 64, nullable: true),
						Email = table.Column<string>(maxLength: 255, nullable: true),
						FullName = table.Column<string>(maxLength: 255, nullable: true),
						InvitationEmailSent = table.Column<bool>(nullable: false, defaultValue: false),
						DeletionHash = table.Column<string>(maxLength: 64, nullable: true)
				},
				constraints : table =>
				{
					table.PrimaryKey("PK_AdvocateApplication", x => x.AdvocateApplicationId);
				});

			migrationBuilder.CreateIndex(
				name: "IX_AdvocateApplication_AdvocateApplicationId",
				table: "AdvocateApplication",
				column: "AdvocateApplicationId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "AdvocateApplication");
		}
	}
}