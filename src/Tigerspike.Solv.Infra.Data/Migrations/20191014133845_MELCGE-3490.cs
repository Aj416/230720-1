using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3490 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdvocateApplicationBrand",
                columns: table => new
                {
                    BrandId = table.Column<Guid>(nullable: false),
                    AdvocateApplicationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvocateApplicationBrand", x => new { x.AdvocateApplicationId, x.BrandId });
                    table.ForeignKey(
                        name: "FK_AdvocateApplicationBrand_AdvocateApplication_AdvocateApplica~",
                        column: x => x.AdvocateApplicationId,
                        principalTable: "AdvocateApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvocateApplicationBrand_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateApplicationBrand_BrandId",
                table: "AdvocateApplicationBrand",
                column: "BrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvocateApplicationBrand");
        }
    }
}
