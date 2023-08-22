using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS21713 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrandFormFieldType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandFormFieldType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrandFormField",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    IsRequired = table.Column<bool>(nullable: false),
                    Validation = table.Column<string>(maxLength: 255, nullable: true),
                    DefaultValue = table.Column<string>(maxLength: 255, nullable: true),
                    Order = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandFormField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandFormField_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandFormField_BrandFormFieldType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "BrandFormFieldType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandFormField_BrandId",
                table: "BrandFormField",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandFormField_Name",
                table: "BrandFormField",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrandFormField_TypeId",
                table: "BrandFormField",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandFormFieldType_Name",
                table: "BrandFormFieldType",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandFormField");

            migrationBuilder.DropTable(
                name: "BrandFormFieldType");
        }
    }
}
