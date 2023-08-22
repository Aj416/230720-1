using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTX2916 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "ClientInvoice",
                type: "decimal(15,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentFailureDate",
                table: "ClientInvoice",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ClientInvoice",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "AdvocateInvoice",
                type: "decimal(15,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentFailureDate",
                table: "AdvocateInvoice",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AdvocateInvoice",
                nullable: false,
                defaultValue: 0);

				
			migrationBuilder.Sql(@"Update AdvocateInvoice A
								Inner Join
								(Select AdvocateInvoiceId,
								Case
									When Sum(P.Amount) = Total THEN 2
									When Sum(P.Amount) > 0 && Sum(P.Amount) < Total THEN 1
								END As Status
								From Payment P
									Inner Join AdvocateInvoiceLineItem On P.AdvocateInvoiceLineItemId = AdvocateInvoiceLineItem.Id
									Inner Join AdvocateInvoice On AdvocateInvoiceLineItem.AdvocateInvoiceId = AdvocateInvoice.Id
								Group By AdvocateInvoiceId,  AdvocateInvoice.Total) As Q On Q.AdvocateInvoiceId = A.Id
								Set A.Status = Q.Status");

			migrationBuilder.Sql(@"Update ClientInvoice C
								Inner Join
								(Select CI.Id ClientInvoiceId,
								Case
									When Sum(P.Amount) = CI.InvoiceTotal THEN 2
									When Sum(P.Amount) > 0 && Sum(P.Amount) < InvoiceTotal THEN 1
								END As Status
								From Payment P
									Inner Join ClientInvoice CI on P.ClientInvoiceId = CI.Id
								Group By CI.Id,  CI.InvoiceTotal) As Q On Q.ClientInvoiceId = C.Id
								Set C.Status = Q.Status;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "ClientInvoice");

            migrationBuilder.DropColumn(
                name: "PaymentFailureDate",
                table: "ClientInvoice");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ClientInvoice");

            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "AdvocateInvoice");

            migrationBuilder.DropColumn(
                name: "PaymentFailureDate",
                table: "AdvocateInvoice");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AdvocateInvoice");
        }
    }
}
