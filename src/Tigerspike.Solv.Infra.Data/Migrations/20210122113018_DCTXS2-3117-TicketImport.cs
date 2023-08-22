using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23117TicketImport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketImport",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(nullable: false),
                    FileType = table.Column<int>(nullable: false),
                    UploadDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    ClosedDate = table.Column<DateTime>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Fee = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketImport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketImportTag",
                columns: table => new
                {
                    TicketImportId = table.Column<Guid>(nullable: false),
                    TagId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketImportTag", x => new { x.TagId, x.TicketImportId });
                    table.ForeignKey(
                        name: "FK_TicketImportTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketImportTag_TicketImport_TicketImportId",
                        column: x => x.TicketImportId,
                        principalTable: "TicketImport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketImportTag_TagId",
                table: "TicketImportTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketImportTag_TicketImportId",
                table: "TicketImportTag",
                column: "TicketImportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketImportTag");

            migrationBuilder.DropTable(
                name: "TicketImport");
        }
    }
}
