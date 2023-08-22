using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22261 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessValue",
                table: "ProfileQuestion",
                nullable: true);

            migrationBuilder.Sql("UPDATE `profilequestion` SET `BusinessValue` = 1 WHERE `Order` = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessValue",
                table: "ProfileQuestion");
        }
    }
}
