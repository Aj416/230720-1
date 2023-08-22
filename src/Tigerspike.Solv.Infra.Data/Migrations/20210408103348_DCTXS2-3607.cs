using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23607 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandAdvocateResponseConfig_Brand_BrandId",
                table: "BrandAdvocateResponseConfig");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "BrandAdvocateResponseConfig",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AddColumn<int>(
                name: "AbandonedCount",
                table: "BrandAdvocateResponseConfig",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EscalationReason",
                table: "BrandAdvocateResponseConfig",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoAbandoned",
                table: "BrandAdvocateResponseConfig",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "BrandAdvocateResponseConfig",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_BrandAdvocateResponseConfig_Brand_BrandId",
                table: "BrandAdvocateResponseConfig",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23607));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandAdvocateResponseConfig_Brand_BrandId",
                table: "BrandAdvocateResponseConfig");

            migrationBuilder.DropColumn(
                name: "AbandonedCount",
                table: "BrandAdvocateResponseConfig");

            migrationBuilder.DropColumn(
                name: "EscalationReason",
                table: "BrandAdvocateResponseConfig");

            migrationBuilder.DropColumn(
                name: "IsAutoAbandoned",
                table: "BrandAdvocateResponseConfig");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "BrandAdvocateResponseConfig");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "BrandAdvocateResponseConfig",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BrandAdvocateResponseConfig_Brand_BrandId",
                table: "BrandAdvocateResponseConfig",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
