using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23149recreateticketfailuretable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketImportFailure",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TicketImportId = table.Column<Guid>(nullable: false),
                    RawInput = table.Column<string>(type: "longtext", nullable: true),
                    FailureReason = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketImportFailure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketImportFailure_TicketImport_TicketImportId",
                        column: x => x.TicketImportId,
                        principalTable: "TicketImport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketImportFailure_TicketImportId",
                table: "TicketImportFailure",
                column: "TicketImportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketImportFailure");
        }
    }
}
