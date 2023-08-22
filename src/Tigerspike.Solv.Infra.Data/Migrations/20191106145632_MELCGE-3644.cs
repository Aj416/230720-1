using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3644 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_ClientInvoice_ReferenceNumber",
                table: "ClientInvoice");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_AdvocateInvoice_ReferenceNumber",
                table: "AdvocateInvoice");

            migrationBuilder.AlterColumn<decimal>(
                name: "VatRate",
                table: "ClientInvoice",
                type: "decimal(6,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "VatAmount",
                table: "ClientInvoice",
                type: "decimal(15,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(15,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "VatRate",
                table: "Brand",
                type: "decimal(6,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,4)",
                oldDefaultValue: 0.2m);

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvoice_ReferenceNumber",
                table: "ClientInvoice",
                column: "ReferenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateInvoice_ReferenceNumber",
                table: "AdvocateInvoice",
                column: "ReferenceNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClientInvoice_ReferenceNumber",
                table: "ClientInvoice");

            migrationBuilder.DropIndex(
                name: "IX_AdvocateInvoice_ReferenceNumber",
                table: "AdvocateInvoice");

            migrationBuilder.AlterColumn<decimal>(
                name: "VatRate",
                table: "ClientInvoice",
                type: "decimal(6,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "VatAmount",
                table: "ClientInvoice",
                type: "decimal(15,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(15,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "VatRate",
                table: "Brand",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0.2m,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,4)",
                oldNullable: true);

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
