using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class MELCGE2709 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsAllRequired",
				table: "ProfileQuestionType",
				nullable: false,
				defaultValue: false);

			migrationBuilder.CreateTable(
				name: "ProfileQuestionOptionDependency",
				columns: table => new
				{
					Id = table.Column<Guid>(nullable: false),
					QuestionOptionId = table.Column<Guid>(nullable: false),
					QuestionOptionDependencyTargetId = table.Column<Guid>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProfileQuestionOptionDependency", x => x.Id);
					table.ForeignKey(
						name: "FK_ProfileQuestionOptionDependency_ProfileQuestionOption_Questi~",
						column: x => x.QuestionOptionId,
						principalTable: "ProfileQuestionOption",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_ProfileQuestionOptionDependency_QuestionOptionId",
				table: "ProfileQuestionOptionDependency",
				column: "QuestionOptionId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "ProfileQuestionOptionDependency");

			migrationBuilder.DropColumn(
				name: "IsAllRequired",
				table: "ProfileQuestionType");
		}
	}
}