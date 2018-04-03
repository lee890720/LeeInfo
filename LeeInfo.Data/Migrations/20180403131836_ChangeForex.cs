using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations
{
    public partial class ChangeForex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Frx_Account",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountNumber = table.Column<string>(nullable: true),
                    AccountPassword = table.Column<string>(nullable: true),
                    Platform = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frx_Account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Frx_History",
                columns: table => new
                {
                    ClosingDealId = table.Column<int>(nullable: false),
                    Balance = table.Column<double>(nullable: false),
                    ClosingPrice = table.Column<double>(nullable: false),
                    ClosingTime = table.Column<DateTime>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    Commissions = table.Column<double>(nullable: false),
                    EntryPrice = table.Column<double>(nullable: false),
                    EntryTime = table.Column<DateTime>(nullable: false),
                    FrxAccountId = table.Column<int>(nullable: true),
                    GrossProfit = table.Column<double>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    NetProfit = table.Column<double>(nullable: false),
                    Pips = table.Column<double>(nullable: false),
                    PositionId = table.Column<int>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    Swap = table.Column<double>(nullable: false),
                    SymbolCode = table.Column<string>(nullable: true),
                    TradeType = table.Column<int>(nullable: false),
                    Volume = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frx_History", x => x.ClosingDealId);
                    table.ForeignKey(
                        name: "FK_Frx_History_Frx_Account_FrxAccountId",
                        column: x => x.FrxAccountId,
                        principalTable: "Frx_Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Frx_Position",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    Commissions = table.Column<double>(nullable: false),
                    EntryTime = table.Column<DateTime>(nullable: false),
                    FrxAccountId = table.Column<int>(nullable: true),
                    GrossProfit = table.Column<double>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    NetProfit = table.Column<double>(nullable: false),
                    Pips = table.Column<double>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    StopLoss = table.Column<double>(nullable: true),
                    Swap = table.Column<double>(nullable: false),
                    SymbolCode = table.Column<string>(nullable: true),
                    TakeProfit = table.Column<double>(nullable: true),
                    TradeType = table.Column<int>(nullable: false),
                    Volume = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frx_Position", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Frx_Position_Frx_Account_FrxAccountId",
                        column: x => x.FrxAccountId,
                        principalTable: "Frx_Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Frx_History_FrxAccountId",
                table: "Frx_History",
                column: "FrxAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Frx_Position_FrxAccountId",
                table: "Frx_Position",
                column: "FrxAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Frx_History");

            migrationBuilder.DropTable(
                name: "Frx_Position");

            migrationBuilder.DropTable(
                name: "Frx_Account");
        }
    }
}
