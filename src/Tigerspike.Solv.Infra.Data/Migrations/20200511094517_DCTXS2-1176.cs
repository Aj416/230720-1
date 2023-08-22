using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS21176 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Contract",
				table: "Brand");

			migrationBuilder.AddColumn<string>(
				name: "ContractUrl",
				table: "Brand",
				maxLength: 256,
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "ContractInternalUrl",
				table: "Brand",
				maxLength: 256,
				nullable: true);

			migrationBuilder.Sql(
				"Update brand Set ContractUrl = concat('https://assets.solvnow.com/', LOWER(Replace(`name`, ' ', '-')), '/contract.txt') Where IsPractice = 0;" +
				"Update brand Set ContractInternalUrl = concat('https://assets.solvnow.com/', LOWER(Replace(`name`, ' ', '-')), '/contract-internal.txt') Where IsPractice = 0;");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "ContractInternalUrl",
				table: "Brand");

			migrationBuilder.DropColumn(
				name: "ContractUrl",
				table: "Brand");

			migrationBuilder.AddColumn<string>(
				name: "Contract",
				table: "Brand",
				type: "longtext",
				nullable: true);
		}
	}
}
