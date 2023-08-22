using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS24010 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.SqlFromFile(nameof(DCTXS24010));

		protected override void Down(MigrationBuilder migrationBuilder)
		{

		}
	}
}
