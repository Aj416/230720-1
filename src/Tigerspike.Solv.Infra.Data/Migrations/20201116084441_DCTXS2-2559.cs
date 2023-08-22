using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22559 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS22559));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
