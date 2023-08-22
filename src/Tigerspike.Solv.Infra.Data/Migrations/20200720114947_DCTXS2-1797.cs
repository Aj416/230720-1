using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS21797 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_User_EscalatedById",
                table: "Ticket");

            migrationBuilder.CreateTable(
                name: "BrandNotificationConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    Type = table.Column<sbyte>(type: "tinyint", nullable: false),
                    DeliverAfterSeconds = table.Column<int>(nullable: false),
                    Subject = table.Column<string>(type: "longtext", nullable: true),
                    Header = table.Column<string>(type: "longtext", nullable: true),
                    Body = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandNotificationConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandNotificationConfig_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandNotificationConfig_BrandId",
                table: "BrandNotificationConfig",
                column: "BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Advocate_EscalatedById",
                table: "Ticket",
                column: "EscalatedById",
                principalTable: "Advocate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Advocate_EscalatedById",
                table: "Ticket");

            migrationBuilder.DropTable(
                name: "BrandNotificationConfig");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_User_EscalatedById",
                table: "Ticket",
                column: "EscalatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
