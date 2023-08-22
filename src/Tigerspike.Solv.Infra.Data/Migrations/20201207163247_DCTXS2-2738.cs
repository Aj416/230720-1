using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22738 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SposDetails",
                table: "Ticket",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SposLead",
                table: "Ticket",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SposEnabled",
                table: "Brand",
                nullable: false,
                defaultValue: false);

			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS22738));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SposDetails",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "SposLead",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "SposEnabled",
                table: "Brand");
        }
    }
}
