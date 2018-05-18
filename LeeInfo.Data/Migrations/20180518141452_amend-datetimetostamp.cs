using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations
{
    public partial class amenddatetimetostamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntryTime",
                table: "Frx_Position");

            migrationBuilder.DropColumn(
                name: "ClosingTime",
                table: "Frx_History");

            migrationBuilder.DropColumn(
                name: "EntryTime",
                table: "Frx_History");

            migrationBuilder.DropColumn(
                name: "ChangeTime",
                table: "Frx_Cashflow");

            migrationBuilder.DropColumn(
                name: "TraderRegistrationTime",
                table: "Frx_Account");

            migrationBuilder.AddColumn<long>(
                name: "EntryTimestamp",
                table: "Frx_Position",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ClosingTimestamp",
                table: "Frx_History",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "EntryTimestamp",
                table: "Frx_History",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ChangeTimestamp",
                table: "Frx_Cashflow",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TraderRegistrationTimestamp",
                table: "Frx_Account",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntryTimestamp",
                table: "Frx_Position");

            migrationBuilder.DropColumn(
                name: "ClosingTimestamp",
                table: "Frx_History");

            migrationBuilder.DropColumn(
                name: "EntryTimestamp",
                table: "Frx_History");

            migrationBuilder.DropColumn(
                name: "ChangeTimestamp",
                table: "Frx_Cashflow");

            migrationBuilder.DropColumn(
                name: "TraderRegistrationTimestamp",
                table: "Frx_Account");

            migrationBuilder.AddColumn<DateTime>(
                name: "EntryTime",
                table: "Frx_Position",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosingTime",
                table: "Frx_History",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EntryTime",
                table: "Frx_History",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangeTime",
                table: "Frx_Cashflow",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TraderRegistrationTime",
                table: "Frx_Account",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
