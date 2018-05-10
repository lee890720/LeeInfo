using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations
{
    public partial class addcashflow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Frx_Cashflow",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false),
                    Balance = table.Column<double>(nullable: false),
                    BalanceVersion = table.Column<long>(nullable: false),
                    ChangeTime = table.Column<DateTime>(nullable: false),
                    Delta = table.Column<double>(nullable: false),
                    Equity = table.Column<double>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frx_Cashflow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Frx_Cashflow_Frx_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Frx_Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Frx_Cashflow_AccountId",
                table: "Frx_Cashflow",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Frx_Cashflow");
        }
    }
}
