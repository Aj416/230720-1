using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23856 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tag",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryDescription",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiagnosisDescription",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SposDescription",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidTransferDescription",
                table: "Brand",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "CategoryDescription",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "DiagnosisDescription",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "SposDescription",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "ValidTransferDescription",
                table: "Brand");
        }
    }
}
