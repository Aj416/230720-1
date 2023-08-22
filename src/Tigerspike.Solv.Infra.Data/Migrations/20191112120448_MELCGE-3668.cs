using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3668 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EscalatedDate",
                table: "Ticket",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EscalationReason",
                table: "Ticket",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EscalatedDate",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "EscalationReason",
                table: "Ticket");
        }
    }
}
