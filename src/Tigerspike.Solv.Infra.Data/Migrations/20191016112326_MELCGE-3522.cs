using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3522 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Induction",
                table: "Brand");

            migrationBuilder.AddColumn<bool>(
                name: "AutomaticAuthorization",
                table: "Brand",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InductionDoneMessage",
                table: "Brand",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InductionInstructions",
                table: "Brand",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnauthorizedMessage",
                table: "Brand",
                type: "longtext",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutomaticAuthorization",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "InductionDoneMessage",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "InductionInstructions",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "UnauthorizedMessage",
                table: "Brand");

            migrationBuilder.AddColumn<string>(
                name: "Induction",
                table: "Brand",
                maxLength: 256,
                nullable: true);
        }
    }
}
