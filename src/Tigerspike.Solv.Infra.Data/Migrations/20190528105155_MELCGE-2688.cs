using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class MELCGE2688 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "AdvocateApplicationId",
				table: "AdvocateApplication",
				newName: "Id");

			migrationBuilder.RenameIndex(
				name: "IX_AdvocateApplication_AdvocateApplicationId",
				table: "AdvocateApplication",
				newName: "IX_AdvocateApplication_Id");

			migrationBuilder.AddColumn<string>(
				name: "Token",
				table: "AdvocateApplication",
				nullable: false);

			// Generate the token column
			migrationBuilder.Sql("Update AdvocateApplication Set Token = TO_BASE64(SHA2(CONCAT(Id, TO_BASE64(HEX(SHA2(CONCAT(NOW(), RAND(), UUID()), 512)))), 256))");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Token",
				table: "AdvocateApplication");

			migrationBuilder.RenameColumn(
				name: "Id",
				table: "AdvocateApplication",
				newName: "AdvocateApplicationId");

			migrationBuilder.RenameIndex(
				name: "IX_AdvocateApplication_Id",
				table: "AdvocateApplication",
				newName: "IX_AdvocateApplication_AdvocateApplicationId");
		}
	}
}
