using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23239 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Brand",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortCode",
                table: "Brand",
                maxLength: 20,
                nullable: true);

			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23239));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "ShortCode",
                table: "Brand");
        }
    }
}
