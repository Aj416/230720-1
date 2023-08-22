using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3637 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Ticket",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TicketSource",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSource", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_SourceId",
                table: "Ticket",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_TicketSource_SourceId",
                table: "Ticket",
                column: "SourceId",
                principalTable: "TicketSource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_TicketSource_SourceId",
                table: "Ticket");

            migrationBuilder.DropTable(
                name: "TicketSource");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_SourceId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Ticket");
        }
    }
}
