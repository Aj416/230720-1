using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS2455 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Authorization",
                table: "WebHook",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "WebHook",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "WebHook",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Verb",
                table: "WebHook",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

			migrationBuilder.Sql("UPDATE WebHook SET ContentType = 'application/json', Verb = 'post'");
			migrationBuilder.Sql(@"UPDATE WebHook SET Body =
'{
  ""eventId"": {{ eventId | json }},
  ""eventType"": {{ eventType | json }},
  ""timestamp"": {{ timestamp | isodate | json }},
  ""data"": {
    ""ticketId"": {{ data.ticketId | json }},
    ""fromStatus"": {{ data.fromStatus | json }},
    ""toStatus"": {{ data.toStatus | json }},
    ""referenceId"": {{ data.referenceId | json }},
    ""source"": {{ data.source | json }}
  }
}'
");

			migrationBuilder.AlterColumn<string>(
				name: "Body",
				table: "WebHook",
				type: "longtext",
				nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Authorization",
                table: "WebHook");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "WebHook");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "WebHook");

            migrationBuilder.DropColumn(
                name: "Verb",
                table: "WebHook");
        }
    }
}
