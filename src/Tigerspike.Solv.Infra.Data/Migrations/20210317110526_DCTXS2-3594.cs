using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23594 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "TicketStatusHistory",
                nullable: false,
                defaultValue: 0);

			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23594));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "TicketStatusHistory");
        }
    }
}
