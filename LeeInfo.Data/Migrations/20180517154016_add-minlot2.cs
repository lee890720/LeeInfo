using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations
{
    public partial class addminlot2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "MinOrderLot",
                table: "Frx_Symbol",
                nullable: false,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "MinOrderLot",
                table: "Frx_Symbol",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
