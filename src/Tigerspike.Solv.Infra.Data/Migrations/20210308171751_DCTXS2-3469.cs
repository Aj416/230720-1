using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23469 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SposNotificationEnabled",
                table: "Tag",
                nullable: true);

			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23469));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SposNotificationEnabled",
                table: "Tag");
        }
    }
}
