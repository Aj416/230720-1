using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22086 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketTag",
                table: "TicketTag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketTag",
                table: "TicketTag",
                columns: new[] { "TagId", "TicketId", "Level" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketTag",
                table: "TicketTag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketTag",
                table: "TicketTag",
                columns: new[] { "TagId", "TicketId" });
        }
    }
}
