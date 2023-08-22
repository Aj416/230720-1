using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3554 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsQuizRequired",
                table: "Brand");

            migrationBuilder.AddColumn<Guid>(
                name: "QuizId",
                table: "Brand",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Quiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    AllowedMistakes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestion",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    QuizId = table.Column<Guid>(nullable: false),
                    IsMultiChoice = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(maxLength: 300, nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestion_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestionOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    QuestionId = table.Column<Guid>(nullable: false),
                    Correct = table.Column<bool>(nullable: false),
                    Text = table.Column<string>(type: "longtext", nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestionOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestionOption_QuizQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuizQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brand_QuizId",
                table: "Brand",
                column: "QuizId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestion_QuizId",
                table: "QuizQuestion",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionOption_QuestionId",
                table: "QuizQuestionOption",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Brand_Quiz_QuizId",
                table: "Brand",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brand_Quiz_QuizId",
                table: "Brand");

            migrationBuilder.DropTable(
                name: "QuizQuestionOption");

            migrationBuilder.DropTable(
                name: "QuizQuestion");

            migrationBuilder.DropTable(
                name: "Quiz");

            migrationBuilder.DropIndex(
                name: "IX_Brand_QuizId",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "Brand");

            migrationBuilder.AddColumn<bool>(
                name: "IsQuizRequired",
                table: "Brand",
                nullable: false,
                defaultValue: false);
        }
    }
}
