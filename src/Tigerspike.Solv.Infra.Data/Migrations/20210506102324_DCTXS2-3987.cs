using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS23987 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "EndChatEnabled",
				table: "Brand",
				nullable: false,
				defaultValue: false);

			migrationBuilder.SqlFromFile(nameof(DCTXS23987));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "EndChatEnabled",
				table: "Brand");
		}
	}
}
