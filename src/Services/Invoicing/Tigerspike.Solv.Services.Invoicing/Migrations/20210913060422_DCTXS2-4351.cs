using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Services.Invoicing.Migrations
{
    public partial class DCTXS24351 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "InvoicingCycle",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    From = table.Column<DateTime>(nullable: false),
                    To = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicingCycle", x => x.Id);
                    table.UniqueConstraint("AK_InvoicingCycle_From", x => x.From);
                    table.UniqueConstraint("AK_InvoicingCycle_To", x => x.To);
                });

            migrationBuilder.CreateTable(
                name: "Sequence",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sequence", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "AdvocateInvoice",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlatformBillingDetailsId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    AdvocateId = table.Column<Guid>(nullable: false),
                    InvoicingCycleId = table.Column<Guid>(nullable: false),
                    Total = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    ReferenceNumber = table.Column<string>(nullable: false),
                    Sequence = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    PaymentFailureDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvocateInvoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvocateInvoice_InvoicingCycle_InvoicingCycleId",
                        column: x => x.InvoicingCycleId,
                        principalTable: "InvoicingCycle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvocateInvoice_BillingDetails_PlatformBillingDetailsId",
                        column: x => x.PlatformBillingDetailsId,
                        principalTable: "BillingDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientInvoice",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Sequence = table.Column<int>(nullable: true),
                    PlatformBillingDetailsId = table.Column<Guid>(nullable: false),
                    BrandBillingDetailsId = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    ReferenceNumber = table.Column<string>(nullable: false),
                    InvoicingCycleId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TicketsCount = table.Column<int>(nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    VatRate = table.Column<decimal>(type: "decimal(6,4)", nullable: true),
                    VatAmount = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    PaymentTotal = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    InvoiceTotal = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    PaymentFailureDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientInvoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientInvoice_BillingDetails_BrandBillingDetailsId",
                        column: x => x.BrandBillingDetailsId,
                        principalTable: "BillingDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientInvoice_InvoicingCycle_InvoicingCycleId",
                        column: x => x.InvoicingCycleId,
                        principalTable: "InvoicingCycle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientInvoice_BillingDetails_PlatformBillingDetailsId",
                        column: x => x.PlatformBillingDetailsId,
                        principalTable: "BillingDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdvocateInvoiceLineItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    AdvocateInvoiceId = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    BrandName = table.Column<string>(nullable: false),
                    TicketsCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvocateInvoiceLineItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvocateInvoiceLineItem_AdvocateInvoice_AdvocateInvoiceId",
                        column: x => x.AdvocateInvoiceId,
                        principalTable: "AdvocateInvoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    ReferenceNumber = table.Column<string>(maxLength: 255, nullable: false),
                    ClientInvoiceId = table.Column<Guid>(nullable: true),
                    AdvocateInvoiceLineItemId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.UniqueConstraint("AK_Payment_ReferenceNumber", x => x.ReferenceNumber);
                    table.ForeignKey(
                        name: "FK_Payment_AdvocateInvoiceLineItem_AdvocateInvoiceLineItemId",
                        column: x => x.AdvocateInvoiceLineItemId,
                        principalTable: "AdvocateInvoiceLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payment_ClientInvoice_ClientInvoiceId",
                        column: x => x.ClientInvoiceId,
                        principalTable: "ClientInvoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoice_AdvocateId",
                table: "AdvocateInvoice",
                column: "AdvocateId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoice_InvoicingCycleId",
                table: "AdvocateInvoice",
                column: "InvoicingCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoice_PlatformBillingDetailsId",
                table: "AdvocateInvoice",
                column: "PlatformBillingDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoice_AdvocateId_ReferenceNumber",
                table: "AdvocateInvoice",
                columns: new[] { "AdvocateId", "ReferenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoice_AdvocateId_Sequence",
                table: "AdvocateInvoice",
                columns: new[] { "AdvocateId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoiceLineItem_AdvocateInvoiceId",
                table: "AdvocateInvoiceLineItem",
                column: "AdvocateInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoiceLineItem_BrandId",
                table: "AdvocateInvoiceLineItem",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvoice_BrandBillingDetailsId",
                table: "ClientInvoice",
                column: "BrandBillingDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvoice_BrandId",
                table: "ClientInvoice",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvoice_InvoicingCycleId",
                table: "ClientInvoice",
                column: "InvoicingCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvoice_PlatformBillingDetailsId",
                table: "ClientInvoice",
                column: "PlatformBillingDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvoice_ReferenceNumber",
                table: "ClientInvoice",
                column: "ReferenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvoice_Sequence",
                table: "ClientInvoice",
                column: "Sequence",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_AdvocateInvoiceLineItemId",
                table: "Payment",
                column: "AdvocateInvoiceLineItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ClientInvoiceId",
                table: "Payment",
                column: "ClientInvoiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Sequence");

            migrationBuilder.DropTable(
                name: "AdvocateInvoiceLineItem");

            migrationBuilder.DropTable(
                name: "ClientInvoice");

            migrationBuilder.DropTable(
                name: "AdvocateInvoice");

            migrationBuilder.DropTable(
                name: "InvoicingCycle");

            migrationBuilder.DropTable(
                name: "BillingDetails");
        }
    }
}
