using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23178 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProbingFormId",
                table: "Brand",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProbingForm",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProbingForm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProbingQuestion",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProbingFormId = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(maxLength: 300, nullable: true),
                    Code = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProbingQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProbingQuestion_ProbingForm_ProbingFormId",
                        column: x => x.ProbingFormId,
                        principalTable: "ProbingForm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProbingQuestionOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    QuestionId = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(maxLength: 300, nullable: true),
                    Action = table.Column<int>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProbingQuestionOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProbingQuestionOption_ProbingQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "ProbingQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProbingResult",
                columns: table => new
                {
                    TicketId = table.Column<Guid>(nullable: false),
                    ProbingQuestionId = table.Column<Guid>(nullable: false),
                    ProbingQuestionOptionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProbingResult", x => new { x.TicketId, x.ProbingQuestionId });
                    table.ForeignKey(
                        name: "FK_ProbingResult_ProbingQuestion_ProbingQuestionId",
                        column: x => x.ProbingQuestionId,
                        principalTable: "ProbingQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProbingResult_ProbingQuestionOption_ProbingQuestionOptionId",
                        column: x => x.ProbingQuestionOptionId,
                        principalTable: "ProbingQuestionOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProbingResult_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brand_ProbingFormId",
                table: "Brand",
                column: "ProbingFormId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProbingQuestion_ProbingFormId",
                table: "ProbingQuestion",
                column: "ProbingFormId");

            migrationBuilder.CreateIndex(
                name: "IX_ProbingQuestionOption_QuestionId",
                table: "ProbingQuestionOption",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProbingResult_ProbingQuestionId",
                table: "ProbingResult",
                column: "ProbingQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProbingResult_ProbingQuestionOptionId",
                table: "ProbingResult",
                column: "ProbingQuestionOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Brand_ProbingForm_ProbingFormId",
                table: "Brand",
                column: "ProbingFormId",
                principalTable: "ProbingForm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

			// run data migration
			migrationBuilder.SqlFromFile(nameof(DCTXS23178));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brand_ProbingForm_ProbingFormId",
                table: "Brand");

            migrationBuilder.DropTable(
                name: "ProbingResult");

            migrationBuilder.DropTable(
                name: "ProbingQuestionOption");

            migrationBuilder.DropTable(
                name: "ProbingQuestion");

            migrationBuilder.DropTable(
                name: "ProbingForm");

            migrationBuilder.DropIndex(
                name: "IX_Brand_ProbingFormId",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "ProbingFormId",
                table: "Brand");
        }
    }
}
