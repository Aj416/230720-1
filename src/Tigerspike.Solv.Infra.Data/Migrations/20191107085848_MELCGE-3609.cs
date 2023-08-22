using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3609 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Advocate",
                nullable: true);

			// script for migrating from AdvocateApplication.Country to columns Advocate.CountryCode
			migrationBuilder.Sql(@"
				UPDATE Advocate a
				INNER JOIN `User` u ON u.Id = a.Id
				INNER JOIN AdvocateApplication app ON app.Email = u.Email
				SET a.CountryCode = app.Country
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClientInvoice_ReferenceNumber",
                table: "ClientInvoice");

            migrationBuilder.DropIndex(
                name: "IX_AdvocateInvoice_ReferenceNumber",
                table: "AdvocateInvoice");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Advocate");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ClientInvoice_ReferenceNumber",
                table: "ClientInvoice",
                column: "ReferenceNumber");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_AdvocateInvoice_ReferenceNumber",
                table: "AdvocateInvoice",
                column: "ReferenceNumber");
        }
    }
}
