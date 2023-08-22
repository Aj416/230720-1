using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23618 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23618));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
