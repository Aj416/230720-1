using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23083 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketCount",
                table: "TicketImport",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TicketImportId1",
                table: "Ticket",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TicketImportId1",
                table: "Ticket",
                column: "TicketImportId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_TicketImport_TicketImportId1",
                table: "Ticket",
                column: "TicketImportId1",
                principalTable: "TicketImport",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_TicketImport_TicketImportId1",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_TicketImportId1",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "TicketCount",
                table: "TicketImport");

            migrationBuilder.DropColumn(
                name: "TicketImportId1",
                table: "Ticket");
        }
    }
}
