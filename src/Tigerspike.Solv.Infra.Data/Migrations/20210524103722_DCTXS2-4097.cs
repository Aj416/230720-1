using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS24097 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusOnPosting",
                table: "BrandAdvocateResponseConfig",
                nullable: true);
            migrationBuilder.SqlFromFile(nameof(DCTXS24097));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusOnPosting",
                table: "BrandAdvocateResponseConfig");
        }
    }
}
