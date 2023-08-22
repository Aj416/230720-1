using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22203 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChatActionId",
                table: "BrandAdvocateResponseConfig",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RelevantTo",
                table: "BrandAdvocateResponseConfig",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChatAction",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    IsBlocking = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatAction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatActionOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ChatActionId = table.Column<Guid>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    IsSuggested = table.Column<bool>(nullable: false)
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
                    Id = table.Column<Guid>(nullable: false),
                    ChatActionOptionId = table.Column<Guid>(nullable: false),
                    Effect = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "ChatActionId",
                table: "BrandAdvocateResponseConfig");

            migrationBuilder.DropColumn(
                name: "RelevantTo",
                table: "BrandAdvocateResponseConfig");
        }
    }
}
