using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23885 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.SqlFromFile(nameof(DCTXS23885)); 
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
