using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22641 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NotificationResumptionState",
                table: "Ticket",
                nullable: false,
                defaultValue: 0);

			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS22641));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationResumptionState",
                table: "Ticket");
        }
    }
}
