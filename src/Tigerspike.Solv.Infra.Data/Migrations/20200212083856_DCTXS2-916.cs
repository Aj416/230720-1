using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS2916 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropForeignKey(
				name: "FK_Payment_AdvocateInvoiceLineItem_AdvocateInvoiceLineItemId",
				table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_AdvocateInvoiceLineItemId",
                table: "Payment");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_AdvocateInvoiceLineItemId",
                table: "Payment",
                column: "AdvocateInvoiceLineItemId",
                unique: true);

			migrationBuilder.AddForeignKey(
				name: "FK_Payment_AdvocateInvoiceLineItem_AdvocateInvoiceLineItemId",
				table: "Payment",
				column: "AdvocateInvoiceLineItemId",
				principalTable: "AdvocateInvoiceLineItem",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payment_AdvocateInvoiceLineItemId",
                table: "Payment");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_AdvocateInvoiceLineItemId",
                table: "Payment",
                column: "AdvocateInvoiceLineItemId");
        }
    }
}
