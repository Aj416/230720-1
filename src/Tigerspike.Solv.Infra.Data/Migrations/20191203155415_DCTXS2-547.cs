using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS2547 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Advocate",
                nullable: true);

			// script for migrating from AdvocateApplication.Source to columns Advocate.Source
			migrationBuilder.Sql(@"
				UPDATE Advocate a
				INNER JOIN `User` u ON u.Id = a.Id
				INNER JOIN AdvocateApplication app ON app.Email = u.Email
				SET a.Source = app.Source
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "Advocate");
        }
    }
}
