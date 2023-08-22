using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS22248 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailureMessage",
                table: "Quiz",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuccessMessage",
                table: "Quiz",
                type: "longtext",
                nullable: true);

             migrationBuilder.Sql("UPDATE quiz q SET FailureMessage = CONCAT('To Solv for ', (SELECT Name FROM brand WHERE QuizId = q.Id LIMIT 1), ' you\\\'re going to need pass the assessment.Hit the button below to brush up on your brand knowledge or try the quiz again.')");
             migrationBuilder.Sql("UPDATE quiz q SET SuccessMessage = CONCAT('You\\\'re now fully authorised with ', (SELECT Name FROM brand WHERE QuizId = q.Id LIMIT 1), ', so go ahead, pick up your first ticket and start earning right now.')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailureMessage",
                table: "Quiz");

            migrationBuilder.DropColumn(
                name: "SuccessMessage",
                table: "Quiz");
        }
    }
}
