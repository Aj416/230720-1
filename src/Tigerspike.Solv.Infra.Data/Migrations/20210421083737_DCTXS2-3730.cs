﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS23730 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        	migrationBuilder.SqlFromFile(nameof(DCTXS23730)); 
		}

		protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
