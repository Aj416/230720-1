using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS21124 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationId",
                table: "ApiKey",
                maxLength: 36,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKey_ApplicationId",
                table: "ApiKey",
                column: "ApplicationId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiKey_ApplicationId",
                table: "ApiKey");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "ApiKey");
        }
    }
}
