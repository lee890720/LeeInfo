using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations
{
    public partial class addfrxsymbol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Frx_Symbol",
                columns: table => new
                {
                    SymbolId = table.Column<int>(nullable: false),
                    AssetClass = table.Column<int>(nullable: false),
                    BaseAsset = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Digits = table.Column<int>(nullable: false),
                    LastAsk = table.Column<double>(nullable: false),
                    LastBid = table.Column<double>(nullable: false),
                    MaxLeverage = table.Column<int>(nullable: false),
                    MaxOrderVolume = table.Column<long>(nullable: false),
                    MeasurementUnits = table.Column<string>(nullable: true),
                    MinOrderStep = table.Column<long>(nullable: false),
                    MinOrderVolume = table.Column<long>(nullable: false),
                    PipPosition = table.Column<int>(nullable: false),
                    QuoteAsset = table.Column<string>(nullable: true),
                    SwapLong = table.Column<double>(nullable: false),
                    SwapShort = table.Column<double>(nullable: false),
                    SymbolName = table.Column<string>(nullable: true),
                    ThreeDaysSwaps = table.Column<string>(nullable: true),
                    TickSize = table.Column<double>(nullable: false),
                    TradeEnabled = table.Column<bool>(nullable: false),
                    TradingMode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frx_Symbol", x => x.SymbolId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Frx_Symbol");
        }
    }
}
