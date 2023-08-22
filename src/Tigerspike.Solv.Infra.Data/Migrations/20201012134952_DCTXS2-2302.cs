using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22302 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE `profilequestion` SET `BusinessValue` = 2 WHERE `Order` = 4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
