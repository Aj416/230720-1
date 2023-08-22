using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23805 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorUserType",
                table: "BrandAdvocateResponseConfig",
                nullable: true);

			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23805));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorUserType",
                table: "BrandAdvocateResponseConfig");
        }
    }
}
