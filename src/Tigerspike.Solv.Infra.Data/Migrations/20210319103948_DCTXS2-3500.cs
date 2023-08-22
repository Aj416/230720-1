using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS23500 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "SolverResponseTimeInSeconds",
				table: "Ticket",
				newName: "SolverTotalResponseTimeInSeconds");

			migrationBuilder.AddColumn<int>(
				name: "CustomerMessageCount",
				table: "Ticket",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<int>(
				name: "SolverMaxResponseTimeInSeconds",
				table: "Ticket",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "SolverMessageCount",
				table: "Ticket",
				nullable: false,
				defaultValue: 0);

			// drop some buggy-leftover column from previous migrations
			migrationBuilder.DropForeignKey("FK_Ticket_TicketImport_TicketImportId1", "Ticket");
			migrationBuilder.DropIndex("IX_Ticket_TicketImportId1", "Ticket");
			migrationBuilder.DropColumn(
				name: "TicketImportId1",
				table: "Ticket");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "CustomerMessageCount",
				table: "Ticket");

			migrationBuilder.DropColumn(
				name: "SolverMaxResponseTimeInSeconds",
				table: "Ticket");

			migrationBuilder.DropColumn(
				name: "SolverMessageCount",
				table: "Ticket");

			migrationBuilder.RenameColumn(
				name: "SolverTotalResponseTimeInSeconds",
				table: "Ticket",
				newName: "SolverResponseTimeInSeconds");
		}
	}
}
