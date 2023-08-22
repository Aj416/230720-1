using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS21720 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CorrectlyDiagnosed",
                table: "Ticket",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EscalatedById",
                table: "Ticket",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_EscalatedById",
                table: "Ticket",
                column: "EscalatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_User_EscalatedById",
                table: "Ticket",
                column: "EscalatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_User_EscalatedById",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_EscalatedById",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "CorrectlyDiagnosed",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "EscalatedById",
                table: "Ticket");
        }
    }
}
