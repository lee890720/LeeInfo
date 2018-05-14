using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations
{
    public partial class amendfrxaccount2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiHost",
                table: "Frx_Account",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiPost",
                table: "Frx_Account",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiUrl",
                table: "Frx_Account",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConnectUrl",
                table: "Frx_Account",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiHost",
                table: "Frx_Account");

            migrationBuilder.DropColumn(
                name: "ApiPost",
                table: "Frx_Account");

            migrationBuilder.DropColumn(
                name: "ApiUrl",
                table: "Frx_Account");

            migrationBuilder.DropColumn(
                name: "ConnectUrl",
                table: "Frx_Account");
        }
    }
}
