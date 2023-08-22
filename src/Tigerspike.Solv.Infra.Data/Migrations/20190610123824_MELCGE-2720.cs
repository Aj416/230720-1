using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE2720 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        // new columns for AdvocateApplication table
	        migrationBuilder.AddColumn<string>(
		        name: "FirstName",
		        table: "AdvocateApplication",
		        maxLength: 200,
		        nullable: false,
		        defaultValue: string.Empty);

	        migrationBuilder.AddColumn<string>(
		        name: "LastName",
		        table: "AdvocateApplication",
		        maxLength: 200,
		        nullable: false,
		        defaultValue: string.Empty);

	        migrationBuilder.AddColumn<string>(
		        name: "Phone",
		        table: "AdvocateApplication",
		        maxLength: 30,
		        nullable: false,
		        defaultValue: string.Empty);

	        // new columns for User table
	        migrationBuilder.AddColumn<string>(
		        name: "Phone",
		        table: "User",
		        maxLength: 30,
		        nullable: true);

	        // script for migrating from AdvocateApplication.FullName to columns AdvocateApplication.FirstName and AdvocateApplication.LastName
	        migrationBuilder.Sql(@"
				UPDATE AdvocateApplication Set
	        	FirstName = IFNULL(TRIM(SUBSTR(FullName, 1, LOCATE(' ', FullName))), ''),
	        	LastName = IFNULL(TRIM(SUBSTR(FullName, LOCATE(' ', FullName))), '')
	        ");

	        // drop no more used AdvocateApplication.FullName column
	        migrationBuilder.DropColumn(
		        name: "FullName",
		        table: "AdvocateApplication");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.AddColumn<string>(
		        name: "FullName",
		        table: "AdvocateApplication",
		        maxLength: 256,
		        nullable: false,
		        defaultValue: string.Empty);

	        // re-population of FullName field is not done by design - there is no real use of creating/maintaing that script

	        migrationBuilder.DropColumn(
		        name: "FirstName",
		        table: "AdvocateApplication");

	        migrationBuilder.DropColumn(
		        name: "LastName",
		        table: "AdvocateApplication");

	        migrationBuilder.DropColumn(
		        name: "Phone",
		        table: "User");
        }
    }
}
