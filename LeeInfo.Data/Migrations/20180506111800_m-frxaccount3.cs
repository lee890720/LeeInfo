using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations
{
    public partial class mfrxaccount3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Frx_UserAccount_Frx_Account_AccountNumber",
                table: "Frx_UserAccount");

            migrationBuilder.DropIndex(
                name: "IX_Frx_UserAccount_AccountNumber",
                table: "Frx_UserAccount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Frx_UserAccount_AccountNumber",
                table: "Frx_UserAccount",
                column: "AccountNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Frx_UserAccount_Frx_Account_AccountNumber",
                table: "Frx_UserAccount",
                column: "AccountNumber",
                principalTable: "Frx_Account",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
