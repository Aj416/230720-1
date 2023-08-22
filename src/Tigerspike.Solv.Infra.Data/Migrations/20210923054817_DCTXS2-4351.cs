using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS24351 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Brand_BillingDetails_BillingDetailsId",
				table: "Brand");

			migrationBuilder.DropForeignKey(
				name: "FK_Ticket_AdvocateInvoice_AdvocateInvoiceId",
				table: "Ticket");

			migrationBuilder.DropForeignKey(
				name: "FK_Ticket_ClientInvoice_ClientInvoiceId",
				table: "Ticket");

			migrationBuilder.DropIndex(
				name: "IX_Ticket_AdvocateInvoiceId",
				table: "Ticket");

			migrationBuilder.DropIndex(
				name: "IX_Ticket_ClientInvoiceId",
				table: "Ticket");

			migrationBuilder.DropIndex(
				name: "IX_Brand_BillingDetailsId",
				table: "Brand");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateIndex(
				name: "IX_Ticket_AdvocateInvoiceId",
				table: "Ticket",
				column: "AdvocateInvoiceId");

			migrationBuilder.CreateIndex(
				name: "IX_Ticket_ClientInvoiceId",
				table: "Ticket",
				column: "ClientInvoiceId");

			migrationBuilder.CreateIndex(
				name: "IX_Brand_BillingDetailsId",
				table: "Brand",
				column: "BillingDetailsId");

			migrationBuilder.AddForeignKey(
				name: "FK_Brand_BillingDetails_BillingDetailsId",
				table: "Brand",
				column: "BillingDetailsId",
				principalTable: "BillingDetails",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_Ticket_AdvocateInvoice_AdvocateInvoiceId",
				table: "Ticket",
				column: "AdvocateInvoiceId",
				principalTable: "AdvocateInvoice",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				name: "FK_Ticket_ClientInvoice_ClientInvoiceId",
				table: "Ticket",
				column: "ClientInvoiceId",
				principalTable: "ClientInvoice",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}
	}
}
