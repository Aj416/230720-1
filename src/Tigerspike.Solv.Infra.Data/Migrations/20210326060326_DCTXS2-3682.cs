using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS23682 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "Order",
				table: "Category",
				nullable: false,
				defaultValue: 0);

			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23682));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Order",
				table: "Category");
		}
	}
}
