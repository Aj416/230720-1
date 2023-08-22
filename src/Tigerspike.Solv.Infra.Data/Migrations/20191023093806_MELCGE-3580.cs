using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3580 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_AdvocateInvoiceLineItem_AdvocateInvoiceLineItemId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_ClientInvoice_ClientInvoiceId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "Receiver",
                table: "Payment");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientInvoiceId",
                table: "Payment",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvocateInvoiceLineItemId",
                table: "Payment",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_AdvocateInvoiceLineItem_AdvocateInvoiceLineItemId",
                table: "Payment",
                column: "AdvocateInvoiceLineItemId",
                principalTable: "AdvocateInvoiceLineItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_ClientInvoice_ClientInvoiceId",
                table: "Payment",
                column: "ClientInvoiceId",
                principalTable: "ClientInvoice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_AdvocateInvoiceLineItem_AdvocateInvoiceLineItemId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_ClientInvoice_ClientInvoiceId",
                table: "Payment");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientInvoiceId",
                table: "Payment",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvocateInvoiceLineItemId",
                table: "Payment",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Receiver",
                table: "Payment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_AdvocateInvoiceLineItem_AdvocateInvoiceLineItemId",
                table: "Payment",
                column: "AdvocateInvoiceLineItemId",
                principalTable: "AdvocateInvoiceLineItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_ClientInvoice_ClientInvoiceId",
                table: "Payment",
                column: "ClientInvoiceId",
                principalTable: "ClientInvoice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
