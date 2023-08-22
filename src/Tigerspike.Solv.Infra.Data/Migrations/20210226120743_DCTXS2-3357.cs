using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS23357 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<bool>(
				name: "Enabled",
				table: "Tag",
				nullable: false,
				defaultValue: true,
				oldClrType: typeof(bool),
				oldType: "tinyint(1)");
		}
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<bool>(
				name: "Enabled",
				table: "Tag",
				type: "tinyint(1)",
				nullable: false,
				oldClrType: typeof(bool),
				oldDefaultValue: true);
		}
	}
}
