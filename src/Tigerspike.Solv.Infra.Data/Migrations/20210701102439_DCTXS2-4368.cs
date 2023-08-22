using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS24368 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AuthorizedDate",
                table: "AdvocateBrand",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractAcceptedDate",
                table: "AdvocateBrand",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InductedDate",
                table: "AdvocateBrand",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizedDate",
                table: "AdvocateBrand");

            migrationBuilder.DropColumn(
                name: "ContractAcceptedDate",
                table: "AdvocateBrand");

            migrationBuilder.DropColumn(
                name: "InductedDate",
                table: "AdvocateBrand");
        }
    }
}
