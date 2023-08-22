using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3449 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_InvoicingCycle_From",
                table: "InvoicingCycle",
                column: "From");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_InvoicingCycle_To",
                table: "InvoicingCycle",
                column: "To");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_InvoicingCycle_From",
                table: "InvoicingCycle");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_InvoicingCycle_To",
                table: "InvoicingCycle");
        }
    }
}
