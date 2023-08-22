using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE2665 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfileArea",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileArea", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileQuestionType",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 300, nullable: false),
                    IsMultiChoice = table.Column<bool>(nullable: false),
                    IsSlider = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileQuestionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileQuestion",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AreaId = table.Column<Guid>(nullable: false),
                    QuestionTypeId = table.Column<Guid>(nullable: false),
                    OrderNumber = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 300, nullable: false),
                    SubTitle = table.Column<string>(maxLength: 300, nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    Optional = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileQuestion_ProfileArea_AreaId",
                        column: x => x.AreaId,
                        principalTable: "ProfileArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileQuestion_ProfileQuestionType_QuestionTypeId",
                        column: x => x.QuestionTypeId,
                        principalTable: "ProfileQuestionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileApplicationAnswer",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AdvocateApplicationId = table.Column<Guid>(nullable: false),
                    QuestionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileApplicationAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileApplicationAnswer_AdvocateApplication_AdvocateApplica~",
                        column: x => x.AdvocateApplicationId,
                        principalTable: "AdvocateApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileApplicationAnswer_ProfileQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "ProfileQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileQuestionOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    QuestionId = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(maxLength: 300, nullable: false),
                    SubText = table.Column<string>(maxLength: 300, nullable: false),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileQuestionOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileQuestionOption_ProfileQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "ProfileQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileAnswer",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ApplicationAnswerId = table.Column<Guid>(nullable: false),
                    QuestionOptionId = table.Column<Guid>(nullable: true),
                    StaticAnswer = table.Column<int>(maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileAnswer_ProfileApplicationAnswer_ApplicationAnswerId",
                        column: x => x.ApplicationAnswerId,
                        principalTable: "ProfileApplicationAnswer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileAnswer_ProfileQuestionOption_QuestionOptionId",
                        column: x => x.QuestionOptionId,
                        principalTable: "ProfileQuestionOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfileQuestionDependency",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    QuestionOptionId = table.Column<Guid>(nullable: false),
                    QuestionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileQuestionDependency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileQuestionDependency_ProfileQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "ProfileQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileQuestionDependency_ProfileQuestionOption_QuestionOpti~",
                        column: x => x.QuestionOptionId,
                        principalTable: "ProfileQuestionOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAnswer_ApplicationAnswerId",
                table: "ProfileAnswer",
                column: "ApplicationAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAnswer_QuestionOptionId",
                table: "ProfileAnswer",
                column: "QuestionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileApplicationAnswer_AdvocateApplicationId",
                table: "ProfileApplicationAnswer",
                column: "AdvocateApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileApplicationAnswer_QuestionId",
                table: "ProfileApplicationAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileQuestion_AreaId",
                table: "ProfileQuestion",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileQuestion_QuestionTypeId",
                table: "ProfileQuestion",
                column: "QuestionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileQuestionDependency_QuestionId",
                table: "ProfileQuestionDependency",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileQuestionDependency_QuestionOptionId",
                table: "ProfileQuestionDependency",
                column: "QuestionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileQuestionOption_QuestionId",
                table: "ProfileQuestionOption",
                column: "QuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfileAnswer");

            migrationBuilder.DropTable(
                name: "ProfileQuestionDependency");

            migrationBuilder.DropTable(
                name: "ProfileApplicationAnswer");

            migrationBuilder.DropTable(
                name: "ProfileQuestionOption");

            migrationBuilder.DropTable(
                name: "ProfileQuestion");

            migrationBuilder.DropTable(
                name: "ProfileArea");

            migrationBuilder.DropTable(
                name: "ProfileQuestionType");
        }
    }
}
