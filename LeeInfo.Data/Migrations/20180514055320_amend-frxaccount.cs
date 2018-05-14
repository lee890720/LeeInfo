using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations
{
    public partial class amendfrxaccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "Frx_Account",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Frx_Account",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "Frx_Account",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Frx_Account",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "Frx_Account");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Frx_Account");

            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "Frx_Account");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Frx_Account");
        }
    }
}
