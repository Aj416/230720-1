using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class MELCGE2790 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerified",
                table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "User",
                nullable: false,
                defaultValue: DateTime.UtcNow);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_User_Email",
                table: "User",
                column: "Email");

            migrationBuilder.CreateTable(
                name: "RejectionReason",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RejectionReason", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Question = table.Column<string>(nullable: false),
                    Complexity = table.Column<sbyte>(type: "tinyint", nullable: true),
                    Csat = table.Column<sbyte>(type: "tinyint", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    AbandonedCount = table.Column<sbyte>(type: "tinyint", nullable: false),
                    RejectionCount = table.Column<sbyte>(type: "tinyint", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    AdvocateId = table.Column<Guid>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    IsPractice = table.Column<bool>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    FirstAssignedDate = table.Column<DateTime>(nullable: true),
                    AssignedDate = table.Column<DateTime>(nullable: true),
                    ClosedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ticket_Advocate_AdvocateId",
                        column: x => x.AdvocateId,
                        principalTable: "Advocate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_User_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketRejectionHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TicketId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketRejectionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketRejectionHistory_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketStatusHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TicketId = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    AdvocateId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketStatusHistory_Advocate_AdvocateId",
                        column: x => x.AdvocateId,
                        principalTable: "Advocate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketStatusHistory_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketRejectionReason",
                columns: table => new
                {
                    TicketRejectionHistoryId = table.Column<Guid>(nullable: false),
                    RejectionReasonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketRejectionReason", x => new { x.TicketRejectionHistoryId, x.RejectionReasonId });
                    table.ForeignKey(
                        name: "FK_TicketRejectionReason_RejectionReason_RejectionReasonId",
                        column: x => x.RejectionReasonId,
                        principalTable: "RejectionReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketRejectionReason_TicketRejectionHistory_TicketRejection~",
                        column: x => x.TicketRejectionHistoryId,
                        principalTable: "TicketRejectionHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_AdvocateId",
                table: "Ticket",
                column: "AdvocateId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_BrandId",
                table: "Ticket",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_CustomerId",
                table: "Ticket",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_Question",
                table: "Ticket",
                column: "Question");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_Status",
                table: "Ticket",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRejectionHistory_TicketId",
                table: "TicketRejectionHistory",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRejectionReason_RejectionReasonId",
                table: "TicketRejectionReason",
                column: "RejectionReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusHistory_AdvocateId",
                table: "TicketStatusHistory",
                column: "AdvocateId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusHistory_TicketId",
                table: "TicketStatusHistory",
                column: "TicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketRejectionReason");

            migrationBuilder.DropTable(
                name: "TicketStatusHistory");

            migrationBuilder.DropTable(
                name: "RejectionReason");

            migrationBuilder.DropTable(
                name: "TicketRejectionHistory");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_User_Email",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddColumn<bool>(
                name: "EmailVerified",
                table: "User",
                nullable: false,
                defaultValue: false);
        }
    }
}
