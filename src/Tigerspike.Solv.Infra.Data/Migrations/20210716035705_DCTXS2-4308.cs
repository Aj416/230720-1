using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS24308 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "L1PostClosureDisable",
                table: "Tag",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "L2PostClosureDisable",
                table: "Tag",
                nullable: false,
                defaultValue: false);
            
            migrationBuilder.SqlFromFile(nameof(DCTXS24308));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "L1PostClosureDisable",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "L2PostClosureDisable",
                table: "Tag");
        }
    }
}
