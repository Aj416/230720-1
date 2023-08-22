using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS23688 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder) =>
			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23688));

		protected override void Down(MigrationBuilder migrationBuilder)
		{

		}
	}
}
