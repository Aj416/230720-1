using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS24168 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AdvocateApplication",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "AdvocateApplication",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "AdvocateApplication",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AdvocateApplication");

            migrationBuilder.DropColumn(
                name: "City",
                table: "AdvocateApplication");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "AdvocateApplication");
        }
    }
}
