using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS22974 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder) =>
			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS22974));

		protected override void Down(MigrationBuilder migrationBuilder)
		{

		}
	}
}
