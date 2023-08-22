using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS21821 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<sbyte>(
                name: "Nps",
                table: "Ticket",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NpsEnabled",
                table: "Brand",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nps",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "NpsEnabled",
                table: "Brand");
        }
    }
}
