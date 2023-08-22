using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS24233 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<Guid>(
				name: "QuestionOptionComboId",
				table: "ProfileQuestionOption",
				nullable: true);

			migrationBuilder.CreateTable(
				name: "ProfileQuestionOptionCombo",
				columns: table => new
				{
					Id = table.Column<Guid>(nullable: false),
					QuestionId = table.Column<Guid>(nullable: false),
					ComboOptionTitle = table.Column<string>(nullable: false),
					ComboOptionType = table.Column<int>(nullable: false),
					Order = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProfileQuestionOptionCombo", x => x.Id);
					table.ForeignKey(
						name: "FK_ProfileQuestionOptionCombo_ProfileQuestion_QuestionId",
						column: x => x.QuestionId,
						principalTable: "ProfileQuestion",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_ProfileQuestionOption_QuestionOptionComboId",
				table: "ProfileQuestionOption",
				column: "QuestionOptionComboId");

			migrationBuilder.CreateIndex(
				name: "IX_ProfileQuestionOptionCombo_QuestionId",
				table: "ProfileQuestionOptionCombo",
				column: "QuestionId");

			migrationBuilder.AddForeignKey(
				name: "FK_ProfileQuestionOption_ProfileQuestionOptionCombo_QuestionOpt~",
				table: "ProfileQuestionOption",
				column: "QuestionOptionComboId",
				principalTable: "ProfileQuestionOptionCombo",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_ProfileQuestionOption_ProfileQuestionOptionCombo_QuestionOpt~",
				table: "ProfileQuestionOption");

			migrationBuilder.DropTable(
				name: "ProfileQuestionOptionCombo");

			migrationBuilder.DropIndex(
				name: "IX_ProfileQuestionOption_QuestionOptionComboId",
				table: "ProfileQuestionOption");

			migrationBuilder.DropColumn(
				name: "QuestionOptionComboId",
				table: "ProfileQuestionOption");
		}
	}
}
