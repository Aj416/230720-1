using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23113 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketImportId",
                table: "Ticket",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TicketImportId",
                table: "Ticket",
                column: "TicketImportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_TicketImport_TicketImportId",
                table: "Ticket",
                column: "TicketImportId",
                principalTable: "TicketImport",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_TicketImport_TicketImportId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_TicketImportId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "TicketImportId",
                table: "Ticket");
        }
    }
}
