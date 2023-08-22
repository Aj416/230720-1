using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22216 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAutoAbandoned",
                table: "AbandonReason",
                nullable: false,
                defaultValue: false);

			// run data migration for new structure
			migrationBuilder.SqlFromFile(nameof(DCTXS22216));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutoAbandoned",
                table: "AbandonReason");
        }
    }
}
