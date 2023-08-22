using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS24177 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<Guid>(
				name: "QuestionOptionComboId",
				table: "ProfileAnswer",
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_ProfileAnswer_QuestionOptionComboId",
				table: "ProfileAnswer",
				column: "QuestionOptionComboId");

			migrationBuilder.AddForeignKey(
				name: "FK_ProfileAnswer_ProfileQuestionOptionCombo_QuestionOptionCombo~",
				table: "ProfileAnswer",
				column: "QuestionOptionComboId",
				principalTable: "ProfileQuestionOptionCombo",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_ProfileAnswer_ProfileQuestionOptionCombo_QuestionOptionCombo~",
				table: "ProfileAnswer");

			migrationBuilder.DropIndex(
				name: "IX_ProfileAnswer_QuestionOptionComboId",
				table: "ProfileAnswer");

			migrationBuilder.DropColumn(
				name: "QuestionOptionComboId",
				table: "ProfileAnswer");
		}
	}
}
