using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23709 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ValidTransfer",
                table: "Ticket",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ValidTransferEnabled",
                table: "Brand",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidTransfer",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "ValidTransferEnabled",
                table: "Brand");
        }
    }
}
