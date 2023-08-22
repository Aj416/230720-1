using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3216 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Invoice_InvoiceId",
                table: "Ticket");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.RenameColumn(
                name: "InvoiceId",
                table: "Ticket",
                newName: "ClientInvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_InvoiceId",
                table: "Ticket",
                newName: "IX_Ticket_ClientInvoiceId");

            migrationBuilder.AddColumn<Guid>(
                name: "AdvocateInvoiceId",
                table: "Ticket",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdvocateInvoice",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    AdvocateId = table.Column<Guid>(nullable: false),
                    InvoicingCycleId = table.Column<Guid>(nullable: false),
                    Total = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    ReferenceNumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvocateInvoice", x => x.Id);
                    table.UniqueConstraint("AK_AdvocateInvoice_ReferenceNumber", x => x.ReferenceNumber);
                    table.ForeignKey(
                        name: "FK_AdvocateInvoice_Advocate_AdvocateId",
                        column: x => x.AdvocateId,
                        principalTable: "Advocate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvocateInvoice_InvoicingCycle_InvoicingCycleId",
                        column: x => x.InvoicingCycleId,
                        principalTable: "InvoicingCycle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientInvoice",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    ReferenceNumber = table.Column<string>(nullable: false),
                    InvoicingCycleId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TicketsCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientInvoice", x => x.Id);
                    table.UniqueConstraint("AK_ClientInvoice_ReferenceNumber", x => x.ReferenceNumber);
                    table.ForeignKey(
                        name: "FK_ClientInvoice_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientInvoice_InvoicingCycle_InvoicingCycleId",
                        column: x => x.InvoicingCycleId,
                        principalTable: "InvoicingCycle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdvocateInvoiceLineItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    AdvocateInvoiceId = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
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
                    table.ForeignKey(
                        name: "FK_AdvocateInvoiceLineItem_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    ReferenceNumber = table.Column<string>(nullable: false),
                    ClientInvoiceId = table.Column<Guid>(nullable: false),
                    AdvocateInvoiceLineItemId = table.Column<Guid>(nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_ClientInvoice_ClientInvoiceId",
                        column: x => x.ClientInvoiceId,
                        principalTable: "ClientInvoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_AdvocateInvoiceId",
                table: "Ticket",
                column: "AdvocateInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoice_AdvocateId",
                table: "AdvocateInvoice",
                column: "AdvocateId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoice_InvoicingCycleId",
                table: "AdvocateInvoice",
                column: "InvoicingCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoiceLineItem_AdvocateInvoiceId",
                table: "AdvocateInvoiceLineItem",
                column: "AdvocateInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoiceLineItem_BrandId",
                table: "AdvocateInvoiceLineItem",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvoice_BrandId",
                table: "ClientInvoice",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvoice_InvoicingCycleId",
                table: "ClientInvoice",
                column: "InvoicingCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_AdvocateInvoiceLineItemId",
                table: "Payment",
                column: "AdvocateInvoiceLineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ClientInvoiceId",
                table: "Payment",
                column: "ClientInvoiceId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_AdvocateInvoice_AdvocateInvoiceId",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_ClientInvoice_ClientInvoiceId",
                table: "Ticket");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "AdvocateInvoiceLineItem");

            migrationBuilder.DropTable(
                name: "ClientInvoice");

            migrationBuilder.DropTable(
                name: "AdvocateInvoice");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_AdvocateInvoiceId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "AdvocateInvoiceId",
                table: "Ticket");

            migrationBuilder.RenameColumn(
                name: "ClientInvoiceId",
                table: "Ticket",
                newName: "InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_ClientInvoiceId",
                table: "Ticket",
                newName: "IX_Ticket_InvoiceId");

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Fee = table.Column<decimal>(nullable: false),
                    InvoicingCycleId = table.Column<Guid>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    ReferenceNumber = table.Column<string>(nullable: true),
                    TicketsCount = table.Column<int>(nullable: false),
                    Total = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoice_InvoicingCycle_InvoicingCycleId",
                        column: x => x.InvoicingCycleId,
                        principalTable: "InvoicingCycle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_InvoicingCycleId",
                table: "Invoice",
                column: "InvoicingCycleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Invoice_InvoiceId",
                table: "Ticket",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
