using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE2614 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 200, nullable: true),
                    LastName = table.Column<string>(maxLength: 200, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    EmailVerified = table.Column<bool>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Advocate",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PaymentMethodSetup = table.Column<bool>(nullable: false),
                    PaymentAccountId = table.Column<string>(maxLength: 255, nullable: true),
                    Csat = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    ShowBrandNotification = table.Column<bool>(nullable: false),
                    VideoWatched = table.Column<bool>(nullable: false),
                    Practicing = table.Column<bool>(nullable: false),
                    PracticeComplete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advocate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Advocate_User_Id",
                        column: x => x.Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Advocate");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
