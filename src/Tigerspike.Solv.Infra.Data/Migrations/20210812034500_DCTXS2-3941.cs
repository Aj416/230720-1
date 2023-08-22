using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23941 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrandMetadataAccess",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    Field = table.Column<string>(nullable: false),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandMetadataAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandMetadataAccess_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandMetadataRoutingConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    Field = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    RouteTo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandMetadataRoutingConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandMetadataRoutingConfig_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandMetadataAccess_BrandId",
                table: "BrandMetadataAccess",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandMetadataRoutingConfig_BrandId",
                table: "BrandMetadataRoutingConfig",
                column: "BrandId",
                unique: true);
            
            migrationBuilder.SqlFromFile(nameof(DCTXS23941));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandMetadataAccess");

            migrationBuilder.DropTable(
                name: "BrandMetadataRoutingConfig");
        }
    }
}
