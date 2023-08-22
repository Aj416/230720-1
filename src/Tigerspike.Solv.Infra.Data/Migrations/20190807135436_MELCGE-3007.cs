using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			// preapre ModifiedDate columns to be not null
			migrationBuilder.Sql("UPDATE `User` SET ModifiedDate = CreatedDate WHERE ModifiedDate IS NULL");
			migrationBuilder.Sql("UPDATE `Ticket` SET ModifiedDate = CreatedDate WHERE ModifiedDate IS NULL");
			migrationBuilder.Sql("UPDATE `Brand` SET ModifiedDate = CreatedDate WHERE ModifiedDate IS NULL");
			migrationBuilder.Sql("UPDATE `AdvocateBrand` SET ModifiedDate = CreatedDate WHERE ModifiedDate IS NULL");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "User",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Ticket",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Brand",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AdvocateBrand",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AdvocateApplication",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvocateBrand_User_AdvocateId",
                table: "AdvocateBrand",
                column: "AdvocateId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvocateBrand_User_AdvocateId",
                table: "AdvocateBrand");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "User",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Ticket",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Brand",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AdvocateBrand",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AdvocateApplication",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)",
                oldClrType: typeof(DateTime));
        }
    }
}
