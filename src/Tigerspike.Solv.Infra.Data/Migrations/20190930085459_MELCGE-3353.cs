using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3353 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Brand");

			migrationBuilder.RenameColumn(
				name: "Total",
				table: "ClientInvoice",
				newName: "Subtotal");

            migrationBuilder.AddColumn<Guid>(
                name: "BrandBillingDetailsId",
                table: "ClientInvoice",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "InvoiceTotal",
                table: "ClientInvoice",
                type: "decimal(15,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentTotal",
                table: "ClientInvoice",
                type: "decimal(15,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "PlatformBillingDetailsId",
                table: "ClientInvoice",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			migrationBuilder.AddColumn<decimal>(
				name: "VatAmount",
				table: "ClientInvoice",
				type: "decimal(15,2)",
				nullable: false,
				defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatRate",
                table: "ClientInvoice",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "BillingDetailsId",
                table: "Brand",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "VatRate",
                table: "Brand",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0.2m);

            migrationBuilder.AddColumn<Guid>(
                name: "PlatformBillingDetailsId",
                table: "AdvocateInvoice",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BillingDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Email = table.Column<string>(maxLength: 255, nullable: true),
                    VatNumber = table.Column<string>(maxLength: 30, nullable: true),
                    CompanyNumber = table.Column<string>(maxLength: 30, nullable: true),
                    Address = table.Column<string>(nullable: true),
                    IsPlatformOwner = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingDetails", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvoice_BrandBillingDetailsId",
                table: "ClientInvoice",
                column: "BrandBillingDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvoice_PlatformBillingDetailsId",
                table: "ClientInvoice",
                column: "PlatformBillingDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Brand_BillingDetailsId",
                table: "Brand",
                column: "BillingDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoice_PlatformBillingDetailsId",
                table: "AdvocateInvoice",
                column: "PlatformBillingDetailsId");

			// add current billing details of the platform
			migrationBuilder.Sql(@"INSERT INTO BillingDetails VALUES(
				'11111111-1111-1111-1111-111111111111',
				'Concentrix',
				'info@solvnow.com',
				'GB 219627101',
				'NI037606',
				'Concentrix Europe Limited \n Mayfield \n 49 East Bridge Street \n Belfast \n BT1 3NR \n United Kingdom',
				1,
				UTC_TIMESTAMP()
			)");
			// add default (empty) billing details for current data
			migrationBuilder.Sql("INSERT INTO BillingDetails (Id, Name, IsPlatformOwner, CreatedDate) VALUES('00000000-0000-0000-0000-000000000000', 'Unspecified',	0, UTC_TIMESTAMP())");

			// update billing details for existing invoices
			migrationBuilder.Sql("UPDATE AdvocateInvoice SET PlatformBillingDetailsId = '11111111-1111-1111-1111-111111111111'");
			migrationBuilder.Sql("UPDATE ClientInvoice SET PlatformBillingDetailsId = '11111111-1111-1111-1111-111111111111'");
			migrationBuilder.Sql("UPDATE ClientInvoice SET InvoiceTotal = Fee");
			migrationBuilder.Sql("UPDATE ClientInvoice SET PaymentTotal = Subtotal");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvocateInvoice_BillingDetails_PlatformBillingDetailsId",
                table: "AdvocateInvoice",
                column: "PlatformBillingDetailsId",
                principalTable: "BillingDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Brand_BillingDetails_BillingDetailsId",
                table: "Brand",
                column: "BillingDetailsId",
                principalTable: "BillingDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientInvoice_BillingDetails_BrandBillingDetailsId",
                table: "ClientInvoice",
                column: "BrandBillingDetailsId",
                principalTable: "BillingDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientInvoice_BillingDetails_PlatformBillingDetailsId",
                table: "ClientInvoice",
                column: "PlatformBillingDetailsId",
                principalTable: "BillingDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvocateInvoice_BillingDetails_PlatformBillingDetailsId",
                table: "AdvocateInvoice");

            migrationBuilder.DropForeignKey(
                name: "FK_Brand_BillingDetails_BillingDetailsId",
                table: "Brand");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientInvoice_BillingDetails_BrandBillingDetailsId",
                table: "ClientInvoice");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientInvoice_BillingDetails_PlatformBillingDetailsId",
                table: "ClientInvoice");

            migrationBuilder.DropTable(
                name: "BillingDetails");

            migrationBuilder.DropIndex(
                name: "IX_ClientInvoice_BrandBillingDetailsId",
                table: "ClientInvoice");

            migrationBuilder.DropIndex(
                name: "IX_ClientInvoice_PlatformBillingDetailsId",
                table: "ClientInvoice");

            migrationBuilder.DropIndex(
                name: "IX_Brand_BillingDetailsId",
                table: "Brand");

            migrationBuilder.DropIndex(
                name: "IX_AdvocateInvoice_PlatformBillingDetailsId",
                table: "AdvocateInvoice");

            migrationBuilder.DropColumn(
                name: "BrandBillingDetailsId",
                table: "ClientInvoice");

            migrationBuilder.DropColumn(
                name: "InvoiceTotal",
                table: "ClientInvoice");

            migrationBuilder.DropColumn(
                name: "PaymentTotal",
                table: "ClientInvoice");

            migrationBuilder.DropColumn(
                name: "PlatformBillingDetailsId",
                table: "ClientInvoice");

			migrationBuilder.DropColumn(
				name: "VatAmount",
				table: "ClientInvoice");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "ClientInvoice");

            migrationBuilder.DropColumn(
                name: "BillingDetailsId",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "PlatformBillingDetailsId",
                table: "AdvocateInvoice");

			migrationBuilder.RenameColumn(
				name: "Subtotal",
				table: "ClientInvoice",
				newName: "Total");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Brand",
                nullable: true);
        }
    }
}
