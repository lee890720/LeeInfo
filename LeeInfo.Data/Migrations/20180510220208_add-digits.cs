using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations
{
    public partial class adddigits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Digits",
                table: "Frx_Position",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Digits",
                table: "Frx_History",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Digits",
                table: "Frx_Position");

            migrationBuilder.DropColumn(
                name: "Digits",
                table: "Frx_History");
        }
    }
}
