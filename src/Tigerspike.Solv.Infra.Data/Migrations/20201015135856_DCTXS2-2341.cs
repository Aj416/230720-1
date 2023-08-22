using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22341 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessValue",
                table: "ProfileQuestion");

            migrationBuilder.AddColumn<int>(
                name: "BusinessValue",
                table: "ProfileQuestionOption",
                nullable: true);

			// run data migration for new structure
			migrationBuilder.SqlFromFile(nameof(DCTXS22341));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessValue",
                table: "ProfileQuestionOption");

            migrationBuilder.AddColumn<int>(
                name: "BusinessValue",
                table: "ProfileQuestion",
                type: "int",
                nullable: true);
        }
    }
}
