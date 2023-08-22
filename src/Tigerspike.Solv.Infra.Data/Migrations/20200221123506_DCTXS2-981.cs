using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS2981 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityApplicantId",
                table: "Advocate",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityCheckId",
                table: "Advocate",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityCheckResultUrl",
                table: "Advocate",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdentityVerificationStatus",
                table: "Advocate",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityApplicantId",
                table: "Advocate");

            migrationBuilder.DropColumn(
                name: "IdentityCheckId",
                table: "Advocate");

            migrationBuilder.DropColumn(
                name: "IdentityCheckResultUrl",
                table: "Advocate");

            migrationBuilder.DropColumn(
                name: "IdentityVerificationStatus",
                table: "Advocate");
        }
    }
}
