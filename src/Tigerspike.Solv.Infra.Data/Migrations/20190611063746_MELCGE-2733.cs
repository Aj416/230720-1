using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE2733 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderNumber",
                table: "ProfileQuestion",
                newName: "Order");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ProfileQuestionOption",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "ProfileQuestionOption");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "ProfileQuestion",
                newName: "OrderNumber");
        }
    }
}
