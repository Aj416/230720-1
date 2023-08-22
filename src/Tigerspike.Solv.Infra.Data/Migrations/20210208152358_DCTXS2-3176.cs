using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23176 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BrandFormField_Name",
                table: "BrandFormField");

            migrationBuilder.AlterColumn<decimal>(
                name: "TicketPrice",
                table: "Brand",
                type: "decimal(15,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(15,2)",
                oldDefaultValue: 3m);

            migrationBuilder.CreateIndex(
                name: "IX_BrandFormField_Name_BrandId",
                table: "BrandFormField",
                columns: new[] { "Name", "BrandId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BrandFormField_Name_BrandId",
                table: "BrandFormField");

            migrationBuilder.AlterColumn<decimal>(
                name: "TicketPrice",
                table: "Brand",
                type: "decimal(15,2)",
                nullable: false,
                defaultValue: 3m,
                oldClrType: typeof(decimal),
                oldType: "decimal(15,2)");

            migrationBuilder.CreateIndex(
                name: "IX_BrandFormField_Name",
                table: "BrandFormField",
                column: "Name",
                unique: true);
        }
    }
}
