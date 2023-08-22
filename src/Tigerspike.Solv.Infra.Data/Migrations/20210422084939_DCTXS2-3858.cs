using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23858 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        	migrationBuilder.SqlFromFile(nameof(DCTXS23858)); 
		}

		protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
