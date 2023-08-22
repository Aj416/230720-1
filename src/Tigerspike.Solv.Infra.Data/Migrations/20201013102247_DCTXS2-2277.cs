using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22277 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilingEmailSent",
                table: "AdvocateApplication");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ProfilingEmailSent",
                table: "AdvocateApplication",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
