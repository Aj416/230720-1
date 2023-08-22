using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS24119 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		=> migrationBuilder.SqlFromFile(nameof(DCTXS24119));

		protected override void Down(MigrationBuilder migrationBuilder)
		{
		}
	}
}
