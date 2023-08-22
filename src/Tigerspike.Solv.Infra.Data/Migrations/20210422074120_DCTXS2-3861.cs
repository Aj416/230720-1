using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23861 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        	migrationBuilder.SqlFromFile(nameof(DCTXS23861)); 
		}

		protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
