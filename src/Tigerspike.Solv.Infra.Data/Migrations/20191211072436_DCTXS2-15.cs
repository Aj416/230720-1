using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
	public partial class DCTXS215 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<decimal>(
				name: "Csat",
				table: "AdvocateBrand",
				type: "decimal(3,2)",
				nullable: false,
				defaultValue : 0m);

			// Calculate the new Advocate Brand Csat value
			migrationBuilder.Sql(
				@"Update AdvocateBrand ab
					Inner Join (Select AVG(Csat) Csat, AdvocateId, BrandId from Ticket where Csat Is Not Null Group By BrandId, AdvocateId) t
					On t.AdvocateId = ab.AdvocateId And t.BrandId = ab.BrandId
				Set ab.Csat = t.Csat");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Csat",
				table: "AdvocateBrand");
		}
	}
}