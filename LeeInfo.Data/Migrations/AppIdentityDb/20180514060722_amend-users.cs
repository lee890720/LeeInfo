using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations.AppIdentityDb
{
    public partial class amendusers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiHost",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiPost",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiUrl",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConnectUrl",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiHost",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApiPost",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApiUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ConnectUrl",
                table: "AspNetUsers");
        }
    }
}
