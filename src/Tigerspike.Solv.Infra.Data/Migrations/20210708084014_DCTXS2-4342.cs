﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS24342 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SqlFromFile(nameof(DCTXS24342));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
