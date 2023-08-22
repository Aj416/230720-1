using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3152 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketPrice");

            migrationBuilder.CreateTable(
                name: "BrandTicketPriceHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    TicketPrice = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    UserId = table.Column<Guid>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandTicketPriceHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandTicketPriceHistory_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandTicketPriceHistory_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandTicketPriceHistory_BrandId",
                table: "BrandTicketPriceHistory",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandTicketPriceHistory_UserId",
                table: "BrandTicketPriceHistory",
                column: "UserId");

			migrationBuilder.Sql("INSERT INTO `BrandTicketPriceHistory` SELECT UUID(), Id, TicketPrice, NULL, UTC_TIMESTAMP() FROM `Brand`");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandTicketPriceHistory");

            migrationBuilder.CreateTable(
                name: "TicketPrice",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(15,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketPrice_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketPrice_BrandId",
                table: "TicketPrice",
                column: "BrandId");
        }
    }
}
