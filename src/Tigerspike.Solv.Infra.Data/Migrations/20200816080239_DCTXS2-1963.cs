using Microsoft.EntityFrameworkCore.Migrations;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS21963 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsBlockedAdvocate",
				table: "AbandonReason",
				nullable: false,
				defaultValue: false);

			// add "Blocked advocate" reasons for all the existing brands
			migrationBuilder.Sql($@"
				INSERT INTO abandonreason (`Id`, `Name`, `IsActive`, `IsBlockedAdvocate`, `BrandId`)
				SELECT UUID(), '{ AbandonReason.BlockedAdvocateReasonName }', true, true, Id FROM brand");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsBlockedAdvocate",
				table: "AbandonReason");
		}
	}
}
