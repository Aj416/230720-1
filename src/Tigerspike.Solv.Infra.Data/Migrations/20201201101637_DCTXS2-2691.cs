using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22691 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "InductionSectionItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "InductionSection",
                nullable: false,
                defaultValue: 0);

			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS22691));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "InductionSectionItem");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "InductionSection");
        }
    }
}
