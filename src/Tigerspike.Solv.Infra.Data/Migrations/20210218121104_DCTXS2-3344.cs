using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23344 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Tag");

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Tag",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Tag",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentTagId",
                table: "Tag",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tag_ParentTagId",
                table: "Tag",
                column: "ParentTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Tag_ParentTagId",
                table: "Tag",
                column: "ParentTagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Tag_ParentTagId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_ParentTagId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "ParentTagId",
                table: "Tag");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Tag",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
