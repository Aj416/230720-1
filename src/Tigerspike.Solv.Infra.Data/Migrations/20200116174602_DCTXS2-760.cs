using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS2760 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "ClientInvoice",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sequence",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sequence", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvoice_Sequence",
                table: "ClientInvoice",
                column: "Sequence",
                unique: true);

            // script for inserting INV prefix into current ReferenceNumber for all ClientInvoices
            migrationBuilder.Sql("UPDATE ClientInvoice SET ReferenceNumber = CONCAT('INV', ReferenceNumber)");

            // script for inserting ClinetInvoice sequnce starting value
            migrationBuilder.Sql("INSERT INTO Sequence VALUES('ClientInvoice', (SELECT COUNT(*) FROM ClientInvoice))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sequence");

            migrationBuilder.DropIndex(
                name: "IX_ClientInvoice_Sequence",
                table: "ClientInvoice");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "ClientInvoice");
        }
    }
}
