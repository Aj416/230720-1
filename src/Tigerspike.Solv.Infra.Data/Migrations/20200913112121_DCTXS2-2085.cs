using Microsoft.EntityFrameworkCore.Migrations;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22085 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "TicketTag",
                nullable: true);

            migrationBuilder.Sql(
                $"UPDATE TicketTag INNER JOIN ticket t on TicketTag.TicketId = t.Id SET TicketTag.Level = t.Level WHERE TicketTag.Level IS NULL");

            migrationBuilder.AlterColumn<string>(name: "Level", table: "TicketTag", nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "TicketTag");
        }
    }
}
