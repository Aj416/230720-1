using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22211 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<sbyte>(
                name: "ReturningCustomerState",
                table: "Ticket",
                type: "tinyint",
                nullable: false,
                defaultValue: (sbyte)0);

            migrationBuilder.AddColumn<bool>(
                name: "IsKey",
                table: "BrandFormField",
                nullable: false,
                defaultValue: false);

			// run data migration for new structure
			migrationBuilder.SqlFromFile(nameof(DCTXS22211));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturningCustomerState",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "IsKey",
                table: "BrandFormField");
        }
    }
}
