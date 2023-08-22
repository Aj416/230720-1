using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS2766 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdvocateInvoice_ReferenceNumber",
                table: "AdvocateInvoice");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sequence",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "AdvocateInvoice",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoice_AdvocateId_ReferenceNumber",
                table: "AdvocateInvoice",
                columns: new[] { "AdvocateId", "ReferenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoice_AdvocateId_Sequence",
                table: "AdvocateInvoice",
                columns: new[] { "AdvocateId", "Sequence" },
                unique: true);

            // script for inserting INV prefix into current ReferenceNumber for all ClientInvoices
            migrationBuilder.Sql("UPDATE AdvocateInvoice SET ReferenceNumber = CONCAT('INV', ReferenceNumber)");

            // script for inserting AdvocateInvoice sequences starting value
            migrationBuilder.Sql(@"
INSERT INTO Sequence (`Name`, `Value`)
SELECT CONCAT('AdvocateInvoice-', AdvocateId) AS `Name`, COUNT(*) AS `Value`
FROM AdvocateInvoice
GROUP BY AdvocateId
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdvocateInvoice_AdvocateId_ReferenceNumber",
                table: "AdvocateInvoice");

            migrationBuilder.DropIndex(
                name: "IX_AdvocateInvoice_AdvocateId_Sequence",
                table: "AdvocateInvoice");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "AdvocateInvoice");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sequence",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoice_ReferenceNumber",
                table: "AdvocateInvoice",
                column: "ReferenceNumber",
                unique: true);
        }
    }
}
