using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS21652 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advocate_AdvocateApplication_Id",
                table: "Advocate");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "User",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "User",
                maxLength: 2,
                nullable: true);

            migrationBuilder.Sql("UPDATE user t1 INNER JOIN advocateApplication t2 ON t1.Id = t2.Id SET t1.Country = t2.Country, t1.State = t2.State");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "User");

            migrationBuilder.DropColumn(
                name: "State",
                table: "User");

            migrationBuilder.AddForeignKey(
                name: "FK_Advocate_AdvocateApplication_Id",
                table: "Advocate",
                column: "Id",
                principalTable: "AdvocateApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
