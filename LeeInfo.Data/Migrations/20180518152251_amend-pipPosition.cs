using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations
{
    public partial class amendpipPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Frx_History",
                newName: "Lot");

            migrationBuilder.RenameColumn(
                name: "Digits",
                table: "Frx_History",
                newName: "PipPosition");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PipPosition",
                table: "Frx_History",
                newName: "Digits");

            migrationBuilder.RenameColumn(
                name: "Lot",
                table: "Frx_History",
                newName: "Quantity");
        }
    }
}
