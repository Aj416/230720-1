using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23522 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BrandFormFieldId",
                table: "TicketMetadataItem",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "TicketMetadataItem",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketMetadataItem_BrandFormFieldId",
                table: "TicketMetadataItem",
                column: "BrandFormFieldId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketMetadataItem_BrandFormField_BrandFormFieldId",
                table: "TicketMetadataItem",
                column: "BrandFormFieldId",
                principalTable: "BrandFormField",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23522));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketMetadataItem_BrandFormField_BrandFormFieldId",
                table: "TicketMetadataItem");

            migrationBuilder.DropIndex(
                name: "IX_TicketMetadataItem_BrandFormFieldId",
                table: "TicketMetadataItem");

            migrationBuilder.DropColumn(
                name: "BrandFormFieldId",
                table: "TicketMetadataItem");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "TicketMetadataItem");
        }
    }
}
