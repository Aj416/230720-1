using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS21478 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhitelistPhrase");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WhitelistPhrase",
                columns: table => new
                {
                    BrandId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Phrase = table.Column<string>(type: "varchar(95) CHARACTER SET utf8mb4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhitelistPhrase", x => new { x.BrandId, x.Phrase });
                    table.ForeignKey(
                        name: "FK_WhitelistPhrase_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
