using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE3516 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgreementContent",
                table: "Brand",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgreementHeading",
                table: "Brand",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAgreementRequired",
                table: "Brand",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsQuizRequired",
                table: "Brand",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AgreementAccepted",
                table: "AdvocateBrand",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgreementContent",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "AgreementHeading",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "IsAgreementRequired",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "IsQuizRequired",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "AgreementAccepted",
                table: "AdvocateBrand");
        }
    }
}
