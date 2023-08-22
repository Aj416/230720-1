using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS27 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentRouteId",
                table: "Brand",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentRoute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRoute", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brand_PaymentRouteId",
                table: "Brand",
                column: "PaymentRouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Brand_PaymentRoute_PaymentRouteId",
                table: "Brand",
                column: "PaymentRouteId",
                principalTable: "PaymentRoute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brand_PaymentRoute_PaymentRouteId",
                table: "Brand");

            migrationBuilder.DropTable(
                name: "PaymentRoute");

            migrationBuilder.DropIndex(
                name: "IX_Brand_PaymentRouteId",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "PaymentRouteId",
                table: "Brand");
        }
    }
}
