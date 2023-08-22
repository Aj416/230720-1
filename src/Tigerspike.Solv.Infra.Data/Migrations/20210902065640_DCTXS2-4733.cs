using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS24733 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.DropTable(name: "Webhook");

	        migrationBuilder.DropForeignKey(
                name: "FK_BrandAdvocateResponseConfig_ChatAction_ChatActionId",
                table: "BrandAdvocateResponseConfig");

            migrationBuilder.DropTable(
                name: "ChatSideEffect");

            migrationBuilder.DropTable(
                name: "ChatActionOption");

            migrationBuilder.DropTable(
                name: "ChatAction");

            migrationBuilder.DropIndex(
                name: "IX_BrandAdvocateResponseConfig_ChatActionId",
                table: "BrandAdvocateResponseConfig");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "Ticket",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "Ticket",
                type: "timestamp(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.CreateTable(
                name: "ChatAction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    IsBlocking = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatAction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatActionOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ChatActionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    IsSuggested = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Label = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Value = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatActionOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatActionOption_ChatAction_ChatActionId",
                        column: x => x.ChatActionId,
                        principalTable: "ChatAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatSideEffect",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ChatActionOptionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Effect = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSideEffect", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatSideEffect_ChatActionOption_ChatActionOptionId",
                        column: x => x.ChatActionOptionId,
                        principalTable: "ChatActionOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandAdvocateResponseConfig_ChatActionId",
                table: "BrandAdvocateResponseConfig",
                column: "ChatActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatActionOption_ChatActionId",
                table: "ChatActionOption",
                column: "ChatActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatSideEffect_ChatActionOptionId",
                table: "ChatSideEffect",
                column: "ChatActionOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandAdvocateResponseConfig_ChatAction_ChatActionId",
                table: "BrandAdvocateResponseConfig",
                column: "ChatActionId",
                principalTable: "ChatAction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
