using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23468 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DiagnosisEnabled",
                table: "Tag",
                nullable: true);

            // run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23468));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiagnosisEnabled",
                table: "Tag");
        }
    }
}
