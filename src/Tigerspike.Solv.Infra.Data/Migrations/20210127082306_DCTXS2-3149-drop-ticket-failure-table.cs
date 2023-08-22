using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23149dropticketfailuretable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketImportFailure");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketImportFailure",
                columns: table => new
                {
                    TicketImportId = table.Column<Guid>(type: "char(36)", nullable: false),
                    MessageBusId = table.Column<Guid>(type: "char(36)", nullable: false),
                    FailureReason = table.Column<string>(type: "longtext", nullable: true),
                    RawInput = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketImportFailure", x => new { x.TicketImportId, x.MessageBusId });
                    table.ForeignKey(
                        name: "FK_TicketImportFailure_TicketImport_TicketImportId",
                        column: x => x.TicketImportId,
                        principalTable: "TicketImport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
