using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdvocateTitle",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateTicketInstructions",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateTicketSubheader",
                table: "Brand",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvocateTitle",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "CreateTicketInstructions",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "CreateTicketSubheader",
                table: "Brand");
        }
    }
}
