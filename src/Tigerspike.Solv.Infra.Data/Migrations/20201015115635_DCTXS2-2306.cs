using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22306 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdvocateQuizAttempt",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    AdvocateId = table.Column<Guid>(nullable: false),
                    QuizId = table.Column<Guid>(nullable: false),
                    Result = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvocateQuizAttempt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvocateQuizAttempt_Advocate_AdvocateId",
                        column: x => x.AdvocateId,
                        principalTable: "Advocate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvocateQuizAttempt_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdvocateQuizAnswer",
                columns: table => new
                {
                    QuizAdvocateAttemptId = table.Column<Guid>(nullable: false),
                    QuizQuestionOptionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvocateQuizAnswer", x => new { x.QuizAdvocateAttemptId, x.QuizQuestionOptionId });
                    table.ForeignKey(
                        name: "FK_AdvocateQuizAnswer_AdvocateQuizAttempt_QuizAdvocateAttemptId",
                        column: x => x.QuizAdvocateAttemptId,
                        principalTable: "AdvocateQuizAttempt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvocateQuizAnswer_QuizQuestionOption_QuizQuestionOptionId",
                        column: x => x.QuizQuestionOptionId,
                        principalTable: "QuizQuestionOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateQuizAnswer_QuizQuestionOptionId",
                table: "AdvocateQuizAnswer",
                column: "QuizQuestionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateQuizAttempt_AdvocateId",
                table: "AdvocateQuizAttempt",
                column: "AdvocateId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvocateQuizAttempt_QuizId",
                table: "AdvocateQuizAttempt",
                column: "QuizId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvocateQuizAnswer");

            migrationBuilder.DropTable(
                name: "AdvocateQuizAttempt");
        }
    }
}
