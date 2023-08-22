using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22890 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockedDate",
                table: "AdvocateBrand");

            migrationBuilder.CreateTable(
                name: "AdvocateBlockHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AdvocateId = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvocateBlockHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvocateBlockHistory_Advocate_AdvocateId",
                        column: x => x.AdvocateId,
                        principalTable: "Advocate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdvocateBlockHistory_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateBlockHistory_AdvocateId",
                table: "AdvocateBlockHistory",
                column: "AdvocateId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateBlockHistory_BrandId",
                table: "AdvocateBlockHistory",
                column: "BrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvocateBlockHistory");

            migrationBuilder.AddColumn<DateTime>(
                name: "BlockedDate",
                table: "AdvocateBrand",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
