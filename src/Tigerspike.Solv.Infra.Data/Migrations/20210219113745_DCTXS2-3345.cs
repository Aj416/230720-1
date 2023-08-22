using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS23345 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<DateTime>(
				name: "CreatedDate",
				table: "TicketTag",
				nullable: true);

			migrationBuilder.AddColumn<Guid>(
				name: "UserId",
				table: "TicketTag",
				nullable: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "CreatedDate",
				table: "TicketTag");

			migrationBuilder.DropColumn(
				name: "UserId",
				table: "TicketTag");
		}
	}
}
