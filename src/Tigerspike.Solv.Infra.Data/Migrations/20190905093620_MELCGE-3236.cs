using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3236 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "ClientInvoice",
                type: "decimal(15,2)",
                nullable: false,
                defaultValue: 0m);

			migrationBuilder.Sql("UPDATE `ClientInvoice` SET Total = Price + Fee");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Total",
                table: "ClientInvoice");
        }
    }
}
