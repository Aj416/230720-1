using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3183 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceId",
                table: "Ticket",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Brand",
                nullable: true);

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
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    ReferenceNumber = table.Column<string>(nullable: true),
                    InvoicingCycleId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    Fee = table.Column<decimal>(nullable: false),
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
                name: "IX_Ticket_InvoiceId",
                table: "Ticket",
                column: "InvoiceId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Invoice_InvoiceId",
                table: "Ticket");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "InvoicingCycle");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_InvoiceId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "Brand");
        }
    }
}
