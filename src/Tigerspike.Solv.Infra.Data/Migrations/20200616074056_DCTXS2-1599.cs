using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS21599 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"DELETE FROM ticketabandonhistory");
			migrationBuilder.Sql(@"DELETE FROM abandonreason");
			migrationBuilder.DropForeignKey("FK_TicketAbandonReason_AbandonReason_AbandonReasonId", "TicketAbandonReason");

            migrationBuilder.AlterColumn<Guid>(
                name: "AbandonReasonId",
                table: "TicketAbandonReason",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AbandonReason",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AbandonReason",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<Guid>(
                name: "BrandId",
                table: "AbandonReason",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AbandonReason",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsForcedEscalation",
                table: "AbandonReason",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AbandonReason_BrandId_Name",
                table: "AbandonReason",
                columns: new[] { "BrandId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AbandonReason_Brand_BrandId",
                table: "AbandonReason",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

			// restore dropped key
			migrationBuilder.AddForeignKey(
				name: "FK_TicketAbandonReason_AbandonReason_AbandonReasonId",
				table: "TicketAbandonReason",
				column: "AbandonReasonId",
				principalTable: "AbandonReason",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			// add "Forced escalation" reasons for all the existing brands
			migrationBuilder.Sql(@"
				INSERT INTO abandonreason (`Id`, `Name`, `IsActive`, `IsForcedEscalation`, `BrandId`)
				SELECT UUID(), '" + AbandonReason.ForcedEscalationReasonName + "', true, true, Id FROM brand");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbandonReason_Brand_BrandId",
                table: "AbandonReason");

            migrationBuilder.DropIndex(
                name: "IX_AbandonReason_BrandId_Name",
                table: "AbandonReason");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "AbandonReason");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AbandonReason");

            migrationBuilder.DropColumn(
                name: "IsForcedEscalation",
                table: "AbandonReason");

            migrationBuilder.AlterColumn<int>(
                name: "AbandonReasonId",
                table: "TicketAbandonReason",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AbandonReason",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AbandonReason",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}
