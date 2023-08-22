using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3657 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketEscalationConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    TicketSourceId = table.Column<int>(nullable: true),
                    OpenTimeInSeconds = table.Column<int>(nullable: true),
                    RejectionCount = table.Column<int>(nullable: true),
                    AbandonedCount = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketEscalationConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketEscalationConfig_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketEscalationConfig_TicketSource_TicketSourceId",
                        column: x => x.TicketSourceId,
                        principalTable: "TicketSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketEscalationConfig_TicketSourceId",
                table: "TicketEscalationConfig",
                column: "TicketSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketEscalationConfig_BrandId_TicketSourceId",
                table: "TicketEscalationConfig",
                columns: new[] { "BrandId", "TicketSourceId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketEscalationConfig");
        }
    }
}
