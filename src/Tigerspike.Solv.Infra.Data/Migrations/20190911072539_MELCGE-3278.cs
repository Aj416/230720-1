using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3278 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "Ticket",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_BrandId_ReferenceId",
                table: "Ticket",
                columns: new[] { "BrandId", "ReferenceId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ticket_BrandId_ReferenceId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Ticket");
        }
    }
}
