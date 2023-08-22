using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS24845 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RedirectAnswer",
                table: "ProbingQuestionOption",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProbingFormRedirectUrl",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SkipCustomerForm",
                table: "Brand",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RedirectAnswer",
                table: "ProbingQuestionOption");

            migrationBuilder.DropColumn(
                name: "ProbingFormRedirectUrl",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "SkipCustomerForm",
                table: "Brand");
        }
    }
}
