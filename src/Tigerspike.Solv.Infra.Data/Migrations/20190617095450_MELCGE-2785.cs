using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE2785 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Thumbnail = table.Column<string>(nullable: true),
                    Logo = table.Column<string>(nullable: true),
                    Contract = table.Column<string>(nullable: true),
                    Induction = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    IsPractice = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdvocateBrand",
                columns: table => new
                {
                    AdvocateId = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    Authorized = table.Column<bool>(nullable: false),
                    ContractAccepted = table.Column<bool>(nullable: false),
                    Inducted = table.Column<bool>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvocateBrand", x => new { x.AdvocateId, x.BrandId });
                    table.ForeignKey(
                        name: "FK_AdvocateBrand_Advocate_AdvocateId",
                        column: x => x.AdvocateId,
                        principalTable: "Advocate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdvocateBrand_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateBrand_BrandId",
                table: "AdvocateBrand",
                column: "BrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvocateBrand");

            migrationBuilder.DropTable(
                name: "Brand");
        }
    }
}
