using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3190 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InductionSection",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InductionSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InductionSection_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InductionSectionItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Source = table.Column<string>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    SectionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InductionSectionItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InductionSectionItem_InductionSection_SectionId",
                        column: x => x.SectionId,
                        principalTable: "InductionSection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InductionAdvocateSectionItem",
                columns: table => new
                {
                    SectionItemId = table.Column<Guid>(nullable: false),
                    AdvocateId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InductionAdvocateSectionItem", x => new { x.AdvocateId, x.SectionItemId });
                    table.ForeignKey(
                        name: "FK_InductionAdvocateSectionItem_Advocate_AdvocateId",
                        column: x => x.AdvocateId,
                        principalTable: "Advocate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InductionAdvocateSectionItem_InductionSectionItem_SectionIte~",
                        column: x => x.SectionItemId,
                        principalTable: "InductionSectionItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InductionAdvocateSectionItem_SectionItemId",
                table: "InductionAdvocateSectionItem",
                column: "SectionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InductionSection_BrandId",
                table: "InductionSection",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_InductionSectionItem_SectionId",
                table: "InductionSectionItem",
                column: "SectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InductionAdvocateSectionItem");

            migrationBuilder.DropTable(
                name: "InductionSectionItem");

            migrationBuilder.DropTable(
                name: "InductionSection");
        }
    }
}
