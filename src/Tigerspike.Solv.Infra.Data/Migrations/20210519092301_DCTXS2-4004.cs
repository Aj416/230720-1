using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS24004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "Ticket",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultCulture",
                table: "Brand",
                maxLength: 10,
                nullable: true);
            migrationBuilder.SqlFromFile(nameof(DCTXS24004));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Culture",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "DefaultCulture",
                table: "Brand");
        }
    }
}
