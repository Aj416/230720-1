using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23499 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BrandFormField",
                type: "longtext",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreateTicketSubheader",
                table: "Brand",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreateTicketInstructions",
                table: "Brand",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateTicketHeader",
                table: "Brand",
                type: "longtext",
                nullable: true);

			migrationBuilder.SqlFromFile(nameof(DCTXS23499));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "BrandFormField");

            migrationBuilder.DropColumn(
                name: "CreateTicketHeader",
                table: "Brand");

            migrationBuilder.AlterColumn<string>(
                name: "CreateTicketSubheader",
                table: "Brand",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreateTicketInstructions",
                table: "Brand",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);
        }
    }
}
