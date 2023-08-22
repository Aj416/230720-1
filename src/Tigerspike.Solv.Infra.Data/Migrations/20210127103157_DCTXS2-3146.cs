using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS23146 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "TicketsImportEnabled",
				table: "Brand",
				nullable: false,
				defaultValue: false);

			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23146));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "TicketsImportEnabled",
				table: "Brand");
		}
	}
}
