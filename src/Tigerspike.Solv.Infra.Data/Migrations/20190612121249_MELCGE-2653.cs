using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE2653 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ProfilingEmailSent",
                table: "AdvocateApplication",
                nullable: false,
                defaultValue: false);

            // set ProfilingEmailSent flag for applications that were already completed
            migrationBuilder.Sql("UPDATE AdvocateApplication SET ProfilingEmailSent = 1 WHERE CompletedEmailSent = 1");

            // remove duplicates, leave the newest
            migrationBuilder.Sql("DELETE t1 FROM AdvocateApplication t1 INNER JOIN AdvocateApplication t2 WHERE t1.CreatedDate < t2.CreatedDate AND t1.Email = t2.Email");

            // final tie breaker - in the unlikely situations where CreatedDate are not distinct for an Email
            migrationBuilder.Sql("DELETE t1 FROM AdvocateApplication t1 INNER JOIN AdvocateApplication t2 WHERE t1.Id < t2.Id AND t1.Email = t2.Email");

			// add unique script for the Email
			migrationBuilder.AddUniqueConstraint(
				name: "UQ_AdvocateApplication_Email",
				table: "AdvocateApplication",
				column: "Email"
			);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilingEmailSent",
                table: "AdvocateApplication");

            migrationBuilder.DropUniqueConstraint(
	            name: "UQ_AdvocateApplication_Email",
	            table: "AdvocateApplication");
        }
    }
}
