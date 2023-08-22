using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS23896 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "AdditionalFeedBackEnabled",
				table: "Brand",
				nullable: false,
				defaultValue: false);

			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23896));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "AdditionalFeedBackEnabled",
				table: "Brand");
		}
	}
}
