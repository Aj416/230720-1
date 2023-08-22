using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23117TicketImportAddNewColumnBrandIdRemoveStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TicketImport");

            migrationBuilder.AddColumn<Guid>(
                name: "BrandId",
                table: "TicketImport",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TicketImport_UserId",
                table: "TicketImport",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketImport_User_UserId",
                table: "TicketImport",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketImport_User_UserId",
                table: "TicketImport");

            migrationBuilder.DropIndex(
                name: "IX_TicketImport_UserId",
                table: "TicketImport");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "TicketImport");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TicketImport",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
