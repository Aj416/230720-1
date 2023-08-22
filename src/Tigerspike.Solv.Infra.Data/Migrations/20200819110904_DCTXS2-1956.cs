using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS21956 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Ticket",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "SuperSolversEnabled",
                table: "Brand",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE ticket SET `Level` = 2 WHERE `Status` < 4 AND `EscalationReason` IS NOT NULL"); // set Level 2 on Escalated tickets
            migrationBuilder.Sql("UPDATE ticket SET `Level` = 2 WHERE `Status` = 4 AND `EscalationReason` IS NOT NULL AND CorrectlyDiagnosed = 0"); // set Level 2 on Closed, but previously Escalated tickets
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "SuperSolversEnabled",
                table: "Brand");
        }
    }
}
