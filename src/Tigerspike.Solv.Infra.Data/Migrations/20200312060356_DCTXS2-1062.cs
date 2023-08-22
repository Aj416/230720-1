using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS21062 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastAdvocateMessageDate",
                table: "Ticket",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCustomerMessageDate",
                table: "Ticket",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAdvocateMessageDate",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "LastCustomerMessageDate",
                table: "Ticket");
        }
    }
}
