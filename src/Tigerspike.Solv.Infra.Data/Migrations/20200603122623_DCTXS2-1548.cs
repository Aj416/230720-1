using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS21548 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbandonReason",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbandonReason", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketAbandonHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TicketId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAbandonHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketAbandonHistory_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketAbandonReason",
                columns: table => new
                {
                    TicketAbandonHistoryId = table.Column<Guid>(nullable: false),
                    AbandonReasonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAbandonReason", x => new { x.TicketAbandonHistoryId, x.AbandonReasonId });
                    table.ForeignKey(
                        name: "FK_TicketAbandonReason_AbandonReason_AbandonReasonId",
                        column: x => x.AbandonReasonId,
                        principalTable: "AbandonReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketAbandonReason_TicketAbandonHistory_TicketAbandonHistor~",
                        column: x => x.TicketAbandonHistoryId,
                        principalTable: "TicketAbandonHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketAbandonHistory_TicketId",
                table: "TicketAbandonHistory",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAbandonReason_AbandonReasonId",
                table: "TicketAbandonReason",
                column: "AbandonReasonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketAbandonReason");

            migrationBuilder.DropTable(
                name: "AbandonReason");

            migrationBuilder.DropTable(
                name: "TicketAbandonHistory");
        }
    }
}
